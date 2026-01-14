using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ControleGastosResiduais.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade Categoria.
/// </summary>
public class CategoriaTests
{
    [Fact]
    public void AceitaTipo_FinalidadeAmbas_DeveAceitarReceita()
    {
        // Arrange
        var categoria = new Categoria { Finalidade = FinalidadeCategoria.Ambas };

        // Act
        var resultado = categoria.AceitaTipo(TipoTransacao.Receita);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void AceitaTipo_FinalidadeAmbas_DeveAceitarDespesa()
    {
        // Arrange
        var categoria = new Categoria { Finalidade = FinalidadeCategoria.Ambas };

        // Act
        var resultado = categoria.AceitaTipo(TipoTransacao.Despesa);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void AceitaTipo_FinalidadeDespesa_DeveAceitarApenasDespesa()
    {
        // Arrange
        var categoria = new Categoria { Finalidade = FinalidadeCategoria.Despesa };

        // Act
        var aceitaDespesa = categoria.AceitaTipo(TipoTransacao.Despesa);
        var aceitaReceita = categoria.AceitaTipo(TipoTransacao.Receita);

        // Assert
        aceitaDespesa.Should().BeTrue();
        aceitaReceita.Should().BeFalse();
    }

    [Fact]
    public void AceitaTipo_FinalidadeReceita_DeveAceitarApenasReceita()
    {
        // Arrange
        var categoria = new Categoria { Finalidade = FinalidadeCategoria.Receita };

        // Act
        var aceitaReceita = categoria.AceitaTipo(TipoTransacao.Receita);
        var aceitaDespesa = categoria.AceitaTipo(TipoTransacao.Despesa);

        // Assert
        aceitaReceita.Should().BeTrue();
        aceitaDespesa.Should().BeFalse();
    }

    [Fact]
    public void Categoria_DeveTerListaDeTransacoesVaziaAoCriar()
    {
        // Arrange & Act
        var categoria = new Categoria();

        // Assert
        categoria.Transacoes.Should().NotBeNull();
        categoria.Transacoes.Should().BeEmpty();
    }

    [Fact]
    public void Categoria_DevePermitirDefinirPropriedades()
    {
        // Arrange & Act
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        // Assert
        categoria.Id.Should().Be(1);
        categoria.Descricao.Should().Be("Alimentação");
        categoria.Finalidade.Should().Be(FinalidadeCategoria.Despesa);
    }
}
