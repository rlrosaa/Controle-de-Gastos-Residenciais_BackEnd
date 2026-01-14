using Microsoft.EntityFrameworkCore;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Infrastructure.Data;

namespace ControleGastosResiduais.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica do repositório base usando Entity Framework Core.
/// </summary>
/// <typeparam name="T">Tipo da entidade.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly GastosResiduaisDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(GastosResiduaisDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            throw new EntidadeNaoEncontradaException(typeof(T).Name, id);
        }
        _dbSet.Remove(entity);
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retorna os totais gerais de todas as transações.
    /// </summary>
    public virtual async Task<TotaisDto> GetTotaisGeraisAsync()
    {
        var totalReceitas = await _context.Transacoes
            .AsNoTracking()
            .Where(t => t.Tipo == TipoTransacao.Receita)
            .SumAsync(t => (decimal?)t.Valor) ?? 0;

        var totalDespesas = await _context.Transacoes
            .AsNoTracking()
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .SumAsync(t => (decimal?)t.Valor) ?? 0;

        return new TotaisDto
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas
        };
    }
}
