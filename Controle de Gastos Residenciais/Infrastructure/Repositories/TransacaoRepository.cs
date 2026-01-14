using Microsoft.EntityFrameworkCore;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Infrastructure.Data;

namespace ControleGastosResiduais.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para a entidade Transacao.
/// </summary>
public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(GastosResiduaisDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retorna todas as transações de uma pessoa específica.
    /// </summary>
    public async Task<IEnumerable<Transacao>> GetByPessoaIdAsync(int pessoaId)
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Where(t => t.PessoaId == pessoaId)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna todas as transações de uma categoria específica.
    /// </summary>
    public async Task<IEnumerable<Transacao>> GetByCategoriaIdAsync(int categoriaId)
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Where(t => t.CategoriaId == categoriaId)
            .ToListAsync();
    }

    /// <summary>
    /// Sobrescreve GetAllAsync para incluir navegações.
    /// </summary>
    public override async Task<IEnumerable<Transacao>> GetAllAsync()
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .ToListAsync();
    }

    /// <summary>
    /// Sobrescreve GetByIdAsync para incluir navegações.
    /// </summary>
    public override async Task<Transacao?> GetByIdAsync(int id)
    {
        return await _context.Transacoes
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// Retorna todas as transações com suas relações (Pessoa e Categoria).
    /// </summary>
    public async Task<IEnumerable<Transacao>> GetAllWithRelationsAsync()
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna uma transação por ID com suas relações (Pessoa e Categoria).
    /// </summary>
    public async Task<Transacao?> GetByIdWithRelationsAsync(int id)
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
