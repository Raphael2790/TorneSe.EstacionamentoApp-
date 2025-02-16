﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TorneSe.EstacionamentoApp.Core.Entidades;

namespace TorneSe.EstacionamentoApp.Data.DAO.Interfaces;

public interface IVeiculoDAO
{
    Task<List<Veiculo>> BuscarVeiculosPorPlaca(string placa);
    Task<bool> ExisteVeiculoComPlaca(string placa);
    Task<int> Inserir(Veiculo veiculo);
}
