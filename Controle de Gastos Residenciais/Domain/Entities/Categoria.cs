using ControleGastosResiduais.Domain.Enums;

namespace ControleGastosResiduais.Domain.Entities;

/// <summary>
/// Representa uma categoria para classificar transações financeiras.
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador único da categoria.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Descrição da categoria (ex: Alimentação, Salário, Lazer).
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Define se a categoria é para despesas, receitas ou ambas.
    /// </summary>
    public FinalidadeCategoria Finalidade { get; set; }

    /// <summary>
    /// Lista de transações associadas a esta categoria.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

    /// <summary>
    /// Verifica se a categoria aceita um determinado tipo de transação.
    /// </summary>
    /// <param name="tipo">Tipo da transação (Despesa ou Receita).</param>
    /// <returns>True se a categoria aceita o tipo, false caso contrário.</returns>
    public bool AceitaTipo(TipoTransacao tipo)
    {
        return Finalidade switch
        {
            FinalidadeCategoria.Ambas => true,
            FinalidadeCategoria.Despesa => tipo == TipoTransacao.Despesa,
            FinalidadeCategoria.Receita => tipo == TipoTransacao.Receita,
            _ => false
        };
    }
}
