using ControleGastosResiduais.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ControleGastosResiduais.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade Pessoa.
/// </summary>
public class PessoaTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(10, true)]
    [InlineData(17, true)]
    [InlineData(18, false)]
    [InlineData(25, false)]
    [InlineData(65, false)]
    public void IsMenorDeIdade_DeveRetornarCorretamenteSeguntoAIdade(int idade, bool esperado)
    {
        // Arrange
        var pessoa = new Pessoa { Idade = idade };

        // Act
        var resultado = pessoa.IsMenorDeIdade();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void Pessoa_DeveTerListaDeTransacoesVaziaAoCriar()
    {
        // Arrange & Act
        var pessoa = new Pessoa();

        // Assert
        pessoa.Transacoes.Should().NotBeNull();
        pessoa.Transacoes.Should().BeEmpty();
    }

    [Fact]
    public void Pessoa_DevePermitirDefinirPropriedades()
    {
        // Arrange & Act
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        // Assert
        pessoa.Id.Should().Be(1);
        pessoa.Nome.Should().Be("João Silva");
        pessoa.Idade.Should().Be(30);
    }
}
