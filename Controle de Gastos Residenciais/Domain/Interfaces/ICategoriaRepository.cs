using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.DTOs;

namespace ControleGastosResiduais.Domain.Interfaces;

/// <summary>
/// Interface para o repositório de Categoria com operações específicas.
/// </summary>
public interface ICategoriaRepository : IRepository<Categoria>
{
    /// <summary>
    /// Retorna os totais de receitas, despesas e saldo de cada categoria.
    /// </summary>
    Task<IEnumerable<CategoriaDto>> GetTotaisPorCategoriaAsync();

    /// <summary>
    /// Retorna uma categoria por ID com suas transações carregadas.
    /// </summary>
    Task<Categoria?> GetByIdComTransacoesAsync(int id);
}
