﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Forms = System.Windows.Forms;
using System;
using TorneSe.EstacionamentoApp.UI.Extensions;
using TorneSe.EstacionamentoApp.Data.Contexto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using TorneSe.EstacionamentoApp.Core.Entidades;
using TorneSe.EstacionamentoApp.Business.Services.Interfaces;
using TorneSe.EstacionamentoApp.Core.Enums;
using TorneSe.EstacionamentoApp.UI.Views;

namespace TorneSe.EstacionamentoApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private readonly IHost _host;
	private Forms.NotifyIcon _notifyIcon;

	public App()
	{
		_host = Host
			.CreateDefaultBuilder()
			.AddDatabase()
			.AddStores()
			.AddNotifications()
            .AddServices()
			.AddBusiness()
			.AddFactories()
			.AddViews()
			.ConfigureHostConfiguration(config =>
			{
				config.SetBasePath(Directory.GetCurrentDirectory());
				config.AddJsonFile("appsettings.json", optional: false);
				config.AddEnvironmentVariables();
			})
			.AddOptions()
            .AddValidators()
			.Build();
	}

    protected override void OnStartup(StartupEventArgs e)
    {
		_host.Start();

		SeedDatabase();

		_notifyIcon = _host.Services.GetRequiredService<Forms.NotifyIcon>();

		//Menus
		_notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
		_notifyIcon.ContextMenuStrip.Items.Add("Sair", null, SairAplicacaoMenuStrip_Click);
		_notifyIcon.Click += NotifyIcon_Click;

		MainWindow = _host.Services.GetRequiredService<LoginWindow>();
		MainWindow.Show();

        base.OnStartup(e);
    }

    private void SeedDatabase()
    {
        var contexto = _host.Services.GetRequiredService<EstacionamentoContexto>();
        var criptografia = _host.Services.GetRequiredService<ICriptografiaService>();

        if (contexto.Database.GetPendingMigrations().Any())
            contexto.Database.Migrate();

        contexto.Database.EnsureCreated();

        CadastrarVagasSeNecessario(contexto);
        CadastrarAdminSeNecessario(contexto, criptografia);
    }

    private void CadastrarAdminSeNecessario(EstacionamentoContexto contexto, ICriptografiaService criptografia)
    {
        if (contexto.Usuarios.Any())
            return;

        var admin = new Usuario()
        {
            Nome = "Administrador",
            Login = "admin",
            Senha = criptografia.CalcularMD5Hash("admin"),
            Ativo = true,
            DataCadastro = DateTime.Now,
            Email = "admin@admin.com",
            TipoUsuario = TipoUsuario.Administrador
        };

        contexto.Usuarios.Add(admin);
        contexto.SaveChanges();
    }

    private static void CadastrarVagasSeNecessario(EstacionamentoContexto contexto)
    {
        if (contexto.Vagas.Any())
            return;

        var vagasPrimeiroAndar = Enumerable.Range(1, 20)
           .Select(i => new Vaga() { Andar = 1, Codigo = "A", Numero = i, Ocupada = false })
           .ToList();

        var vagasSegundoAndar = Enumerable.Range(1, 15)
            .Select(i => new Vaga() { Andar = 2, Codigo = "B", Numero = i, Ocupada = false })
            .ToList();

        contexto.Vagas.AddRange(vagasPrimeiroAndar);
        contexto.Vagas.AddRange(vagasSegundoAndar);
        contexto.SaveChanges();
    }

    private void NotifyIcon_Click(object? sender, EventArgs e)
    {
        MainWindow.WindowState = WindowState.Normal;
		MainWindow.Activate();
    }

    private void SairAplicacaoMenuStrip_Click(object? sender, EventArgs e) => Shutdown();

    protected override void OnExit(ExitEventArgs e)
    {
		_host.Dispose();

		_notifyIcon.Dispose();

        base.OnExit(e);
    }
}
