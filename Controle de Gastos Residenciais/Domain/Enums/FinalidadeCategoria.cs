namespace ControleGastosResiduais.Domain.Enums;

/// <summary>
/// Define a finalidade de uma categoria de transação.
/// </summary>
public enum FinalidadeCategoria
{
    /// <summary>
    /// Categoria exclusiva para despesas.
    /// </summary>
    Despesa = 0,

    /// <summary>
    /// Categoria exclusiva para receitas.
    /// </summary>
    Receita = 1,

    /// <summary>
    /// Categoria que pode ser usada tanto para despesas quanto receitas.
    /// </summary>
    Ambas = 2
}
