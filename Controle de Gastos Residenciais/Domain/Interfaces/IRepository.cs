using ControleGastosResiduais.Domain.DTOs;

namespace ControleGastosResiduais.Domain.Interfaces;

/// <summary>
/// Interface base para repositórios genéricos.
/// </summary>
/// <typeparam name="T">Tipo da entidade.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Busca uma entidade por ID.
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Retorna todas as entidades.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Remove uma entidade por ID.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Salva as mudanças no contexto.
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Retorna os totais gerais de todas as transações.
    /// </summary>
    Task<TotaisDto> GetTotaisGeraisAsync();
}
