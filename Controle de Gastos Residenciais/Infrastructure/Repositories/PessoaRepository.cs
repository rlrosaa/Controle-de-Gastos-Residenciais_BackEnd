using Microsoft.EntityFrameworkCore;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Infrastructure.Data;

namespace ControleGastosResiduais.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para a entidade Pessoa.
/// </summary>
public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
{
    public PessoaRepository(GastosResiduaisDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retorna os totais de receitas, despesas e saldo de cada pessoa.
    /// </summary>
    public async Task<IEnumerable<PessoaDto>> GetTotaisPorPessoaAsync()
    {
        return await _context.Pessoas
            .AsNoTracking()
            .Select(p => new PessoaDto
            {
                Id = p.Id.GetValueOrDefault(),
                Nome = p.Nome,
                Idade = p.Idade,
                TotalReceitas = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => (decimal?)t.Valor) ?? 0,
                TotalDespesas = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => (decimal?)t.Valor) ?? 0
            })
            .ToListAsync();
    }
}
