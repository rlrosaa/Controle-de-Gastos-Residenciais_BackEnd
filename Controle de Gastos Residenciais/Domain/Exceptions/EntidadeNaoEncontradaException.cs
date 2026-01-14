namespace ControleGastosResiduais.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma entidade não é encontrada.
/// </summary>
public class EntidadeNaoEncontradaException : DomainException
{
    public EntidadeNaoEncontradaException(string entidade, int id) 
        : base($"{entidade} com ID {id} não foi encontrada.")
    {
    }
}
