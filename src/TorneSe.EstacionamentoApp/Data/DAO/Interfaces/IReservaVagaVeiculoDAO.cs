﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TorneSe.EstacionamentoApp.Core.Comum;
using TorneSe.EstacionamentoApp.Core.Entidades;
using TorneSe.EstacionamentoApp.Data.Dtos;

namespace TorneSe.EstacionamentoApp.Data.DAO.Interfaces;

public interface IReservaVagaVeiculoDAO
{
    Task Atualizar(ReservaVagaVeiculo reservaVaga, ResumoSaida resumoSaida);
    Task Inserir(ReservaVagaVeiculo reservaVagaVeiculo);
    Task<List<ReservaVagaFormaPagamentoDto>> ObterFaturamentoPorFormaPagamento();
    Task<List<ReservaVagaFaturamentoDto>> ObterFaturamentoPorMes();
    Task<ReservaVagaVeiculo?> ObterReservaVagaVeiculo(int idVeiculo, int idVaga);
}
