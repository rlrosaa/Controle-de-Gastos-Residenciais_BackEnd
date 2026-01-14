namespace ControleGastosResiduais.Domain.DTOs;

/// <summary>
/// DTO para retorno da listagem de pessoas com totais gerais.
/// </summary>
public class ListaPessoasDto
{
    public IEnumerable<PessoaDto> Pessoas { get; set; } = Enumerable.Empty<PessoaDto>();
    public TotaisDto TotaisGerais { get; set; } = new();
}
