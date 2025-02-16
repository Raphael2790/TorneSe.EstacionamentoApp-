﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TorneSe.EstacionamentoApp.Business.Interfaces;
using TorneSe.EstacionamentoApp.Componentes;
using TorneSe.EstacionamentoApp.UI.Args;
using TorneSe.EstacionamentoApp.UI.Notifications.Interfaces;
using TorneSe.EstacionamentoApp.UI.Store;

namespace TorneSe.EstacionamentoApp.Views;

/// <summary>
/// Interação lógica para EntradaVeiculosView.xam
/// </summary>
public partial class EntradaVeiculosView : UserControl
{
    private readonly VagasStore _veiculosStore;
    private readonly IVeiculoBusiness _veiculoBusiness;
    private readonly INotificationService _notificationService;

    private int _pagina = 1;
    private const int _paginaInicial = 1;
    private int _porPagina = 20;
    private int _totalPaginas = 0;

    private const string _componente = "Entrada";

    public EntradaVeiculosView(VagasStore veiculosStore, 
                               IVeiculoBusiness veiculoBusiness,
                               INotificationService notificationService)
    {
        InitializeComponent();
        _veiculosStore = veiculosStore;
        _totalPaginas = (int)Math.Ceiling(_veiculosStore.VagasLivres.Count / (double)_porPagina);
        _veiculoBusiness = veiculoBusiness;
        _notificationService = notificationService;
        MontarComponente();
        veiculosStore.StoreChanged += VeiculosStore_StoreChanged;
    }

    private void VeiculosStore_StoreChanged(object? sender, VagasStoreEventArgs e)
    {
        _notificationService.Notificar(1000, "Entrada Veiculo Realizada", $"A vaga {e.ResumoVaga.NomeVaga} foi ocupada!");
        MontarComponente();
    }

    private void MontarComponente()
    {
        var vagas = _veiculosStore.VagasLivres.Skip((_pagina - 1) * _porPagina).Take(_porPagina).ToList();

        vagasControl.Content = new VagasGridControl(vagas, _componente, _veiculoBusiness, _veiculosStore, null!);
        buscaVagaTextBox.IsEnabled = true;
        voltarButton.Visibility = Visibility.Visible;
        proximoButton.Visibility = Visibility.Visible;
    }

    private async void ProximasVagas_ButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_pagina == _totalPaginas)
        {
            MessageBox.Show("Não há mais vagas a serem carregadas");
            return;
        }

        vagasControl.Content = new LoadingSquareControl();

        buscaVagaTextBox.IsEnabled = false;
        voltarButton.Visibility = Visibility.Hidden;
        proximoButton.Visibility = Visibility.Hidden;

        await Task.Delay(3000);

        _pagina += 1;
        MontarComponente();
    }

    private async void VoltarVagas_ButtonClick(object sender, RoutedEventArgs e)
    {
        if (_pagina is _paginaInicial)
            return;

        vagasControl.Content = new LoadingSquareControl();

        buscaVagaTextBox.IsEnabled = false;
        voltarButton.Visibility = Visibility.Hidden;
        proximoButton.Visibility = Visibility.Hidden;

        await Task.Delay(3000);

        _pagina -= 1;
        MontarComponente();
    }

    private void VagaBuscaTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textbox)
            return;

        var vagas = _veiculosStore.VagasLivres.Where(v => v.NomeVaga.Contains(textbox.Text)).ToList();

        if (!vagas.Any())
        {
            MessageBox.Show("Não há vagas com esse nome");
            return;
        }

        vagasControl.Content = new VagasGridControl(vagas, _componente, _veiculoBusiness, _veiculosStore, null!);
    }
}
