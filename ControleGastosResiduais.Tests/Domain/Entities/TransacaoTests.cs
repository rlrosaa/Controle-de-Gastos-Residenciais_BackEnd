using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ControleGastosResiduais.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade Transacao.
/// </summary>
public class TransacaoTests
{
    [Fact]
    public void Transacao_DevePermitirDefinirPropriedades()
    {
        // Arrange & Act
        var transacao = new Transacao
        {
            Id = 1,
            Descricao = "Compra de supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        // Assert
        transacao.Id.Should().Be(1);
        transacao.Descricao.Should().Be("Compra de supermercado");
        transacao.Valor.Should().Be(150.50m);
        transacao.Tipo.Should().Be(TipoTransacao.Despesa);
        transacao.CategoriaId.Should().Be(1);
        transacao.PessoaId.Should().Be(1);
    }

    [Fact]
    public void Transacao_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var transacao = new Transacao();

        // Assert
        transacao.Descricao.Should().BeEmpty();
        transacao.Valor.Should().Be(0);
        transacao.Categoria.Should().BeNull();
        transacao.Pessoa.Should().BeNull();
    }

    [Fact]
    public void Transacao_DeveAceitarRelacionamentos()
    {
        // Arrange
        var categoria = new Categoria { Id = 1, Descricao = "Alimentação" };
        var pessoa = new Pessoa { Id = 1, Nome = "João Silva" };

        // Act
        var transacao = new Transacao
        {
            Categoria = categoria,
            Pessoa = pessoa
        };

        // Assert
        transacao.Categoria.Should().NotBeNull();
        transacao.Categoria.Descricao.Should().Be("Alimentação");
        transacao.Pessoa.Should().NotBeNull();
        transacao.Pessoa.Nome.Should().Be("João Silva");
    }
}
