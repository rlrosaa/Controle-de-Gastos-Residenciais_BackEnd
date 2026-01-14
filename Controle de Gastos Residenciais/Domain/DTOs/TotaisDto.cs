namespace ControleGastosResiduais.Domain.DTOs;

/// <summary>
/// DTO com totais gerais consolidados.
/// </summary>
public class TotaisDto
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal SaldoLiquido => TotalReceitas - TotalDespesas;
}
