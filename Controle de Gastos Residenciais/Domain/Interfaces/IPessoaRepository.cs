using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.DTOs;

namespace ControleGastosResiduais.Domain.Interfaces;

/// <summary>
/// Interface para o repositório de Pessoa com operações específicas.
/// </summary>
public interface IPessoaRepository : IRepository<Pessoa>
{
    /// <summary>
    /// Retorna os totais de receitas, despesas e saldo de cada pessoa.
    /// </summary>
    Task<IEnumerable<PessoaDto>> GetTotaisPorPessoaAsync();
}
