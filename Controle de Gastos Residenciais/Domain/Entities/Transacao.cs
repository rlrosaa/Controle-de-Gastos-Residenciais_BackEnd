using ControleGastosResiduais.Domain.Enums;

namespace ControleGastosResiduais.Domain.Entities;

/// <summary>
/// Representa uma movimentação financeira (despesa ou receita).
/// </summary>
public class Transacao
{
    /// <summary>
    /// Identificador único da transação.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Descrição da transação.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor da transação (sempre positivo).
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Tipo da transação (Despesa ou Receita).
    /// </summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>
    /// Identificador da categoria associada.
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// Categoria associada à transação.
    /// </summary>
    public Categoria? Categoria { get; set; }

    /// <summary>
    /// Identificador da pessoa que realizou a transação.
    /// </summary>
    public int PessoaId { get; set; }

    /// <summary>
    /// Pessoa que realizou a transação.
    /// </summary>
    public Pessoa? Pessoa { get; set; }
}
