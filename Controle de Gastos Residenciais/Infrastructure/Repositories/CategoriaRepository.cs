using Microsoft.EntityFrameworkCore;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Infrastructure.Data;

namespace ControleGastosResiduais.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para a entidade Categoria.
/// </summary>
public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(GastosResiduaisDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retorna uma categoria por ID com suas transações carregadas.
    /// </summary>
    public async Task<Categoria?> GetByIdComTransacoesAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Transacoes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Retorna os totais de receitas, despesas e saldo de cada categoria.
    /// </summary>
    public async Task<IEnumerable<CategoriaDto>> GetTotaisPorCategoriaAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Select(c => new CategoriaDto
            {
                Id = c.Id.GetValueOrDefault(),
                Descricao = c.Descricao,
                Finalidade = c.Finalidade,
                TotalReceitas = c.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => (decimal?)t.Valor) ?? 0,
                TotalDespesas = c.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => (decimal?)t.Valor) ?? 0
            })
            .ToListAsync();
    }
}
