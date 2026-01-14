namespace ControleGastosResiduais.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// </summary>
public class ValidacaoNegocioException : DomainException
{
    public ValidacaoNegocioException(string message) : base(message)
    {
    }
}
