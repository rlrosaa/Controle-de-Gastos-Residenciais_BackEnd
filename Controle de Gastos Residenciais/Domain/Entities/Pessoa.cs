namespace ControleGastosResiduais.Domain.Entities;

/// <summary>
/// Representa uma pessoa que realiza transações financeiras.
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Identificador único da pessoa.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Idade da pessoa em anos.
    /// </summary>
    public int Idade { get; set; }

    /// <summary>
    /// Lista de transações realizadas pela pessoa.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

    /// <summary>
    /// Verifica se a pessoa é menor de idade (menor de 18 anos).
    /// </summary>
    /// <returns>True se a pessoa tem menos de 18 anos, false caso contrário.</returns>
    public bool IsMenorDeIdade() => Idade < 18;
}
