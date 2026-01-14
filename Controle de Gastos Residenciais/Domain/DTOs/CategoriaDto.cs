using ControleGastosResiduais.Domain.Enums;

namespace ControleGastosResiduais.Domain.DTOs;

/// <summary>
/// DTO com dados de categoria incluindo totais de receitas, despesas e saldo.
/// </summary>
public class CategoriaDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public FinalidadeCategoria Finalidade { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
