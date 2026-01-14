using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Domain.Exceptions;

namespace ControleGastosResiduais.Domain.Services;

/// <summary>
/// Serviço de domínio responsável por validações de regras de negócio relacionadas a transações.
/// </summary>
public class TransacaoService
{
    /// <summary>
    /// Valida uma transação antes de criá-la, aplicando as regras de negócio.
    /// </summary>
    /// <param name="transacao">Transação a ser validada.</param>
    /// <param name="pessoa">Pessoa associada à transação.</param>
    /// <param name="categoria">Categoria associada à transação.</param>
    /// <exception cref="ValidacaoNegocioException">Lançada quando uma regra de negócio é violada.</exception>
    public void ValidarTransacao(Transacao transacao, Pessoa pessoa, Categoria categoria)
    {
        if (!categoria.AceitaTipo(transacao.Tipo))
        {
            throw new ValidacaoNegocioException(
                $"A categoria '{categoria.Descricao}' não permite transações do tipo '{transacao.Tipo}'.");
        }

        if (pessoa.IsMenorDeIdade() && transacao.Tipo == TipoTransacao.Receita)
        {
            throw new ValidacaoNegocioException(
                "Menores de idade não podem criar transações de receita.");
        }
    }
}
