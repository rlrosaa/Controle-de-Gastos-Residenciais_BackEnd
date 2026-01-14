namespace ControleGastosResiduais.Domain.DTOs;

/// <summary>
/// DTO para retorno da listagem de categorias com totais gerais.
/// </summary>
public class ListaCategoriasDto
{
    public IEnumerable<CategoriaDto> Categorias { get; set; } = Enumerable.Empty<CategoriaDto>();
    public TotaisDto TotaisGerais { get; set; } = new();
}
