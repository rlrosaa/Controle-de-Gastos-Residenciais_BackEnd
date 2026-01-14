namespace ControleGastosResiduais.Domain.DTOs;

/// <summary>
/// DTO com dados de pessoa incluindo totais de receitas, despesas e saldo.
/// </summary>
public class PessoaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
