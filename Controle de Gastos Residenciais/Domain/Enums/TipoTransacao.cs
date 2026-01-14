namespace ControleGastosResiduais.Domain.Enums;

/// <summary>
/// Define o tipo de uma transação financeira.
/// </summary>
public enum TipoTransacao
{
    /// <summary>
    /// Transação de saída de dinheiro (gasto).
    /// </summary>
    Despesa = 0,

    /// <summary>
    /// Transação de entrada de dinheiro (ganho).
    /// </summary>
    Receita = 1
}
