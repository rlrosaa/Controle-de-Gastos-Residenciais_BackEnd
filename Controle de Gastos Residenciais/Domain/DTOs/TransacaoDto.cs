using ControleGastosResiduais.Domain.Enums;

namespace ControleGastosResiduais.Domain.DTOs;

/// <summary>
/// DTO de retorno de transação.
/// </summary>
public class TransacaoDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public int PessoaId { get; set; }
    public string PessoaNome { get; set; } = string.Empty;
}
