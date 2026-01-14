using ControleGastosResiduais.Domain.Entities;

namespace ControleGastosResiduais.Domain.Interfaces;

/// <summary>
/// Interface para o repositório de Transacao com operações específicas.
/// </summary>
public interface ITransacaoRepository : IRepository<Transacao>
{
    /// <summary>
    /// Retorna todas as transações de uma pessoa específica.
    /// </summary>
    Task<IEnumerable<Transacao>> GetByPessoaIdAsync(int pessoaId);

    /// <summary>
    /// Retorna todas as transações de uma categoria específica.
    /// </summary>
    Task<IEnumerable<Transacao>> GetByCategoriaIdAsync(int categoriaId);

    /// <summary>
    /// Retorna todas as transações com suas relações (Pessoa e Categoria).
    /// </summary>
    Task<IEnumerable<Transacao>> GetAllWithRelationsAsync();

    /// <summary>
    /// Retorna uma transação por ID com suas relações (Pessoa e Categoria).
    /// </summary>
    Task<Transacao?> GetByIdWithRelationsAsync(int id);
}
