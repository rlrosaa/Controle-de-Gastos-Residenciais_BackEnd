using ControleGastosResiduais.Application.Services;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGastosResiduais.Tests.Application.Services;

/// <summary>
/// Testes unitários para CategoriaAppService.
/// </summary>
public class CategoriaAppServiceTests
{
    private readonly Mock<ICategoriaRepository> _repositoryMock;
    private readonly Mock<ILogger<CategoriaAppService>> _loggerMock;
    private readonly CategoriaAppService _service;

    public CategoriaAppServiceTests()
    {
        _repositoryMock = new Mock<ICategoriaRepository>();
        _loggerMock = new Mock<ILogger<CategoriaAppService>>();
        _service = new CategoriaAppService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ObterEntidadePorIdAsync_CategoriaExiste_DeveRetornarCategoria()
    {
        // Arrange
        var categoriaEsperada = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        _repositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(1))
            .ReturnsAsync(categoriaEsperada);

        // Act
        var resultado = await _service.ObterEntidadePorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(categoriaEsperada);
    }

    [Fact]
    public async Task ObterEntidadePorIdAsync_CategoriaNaoExiste_DeveLancarExcecao()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(999))
            .ReturnsAsync((Categoria?)null);

        // Act
        var act = async () => await _service.ObterEntidadePorIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage("*Categoria*999*");
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRetornarListaComTotais()
    {
        // Arrange
        var categorias = new List<CategoriaDto>
        {
            new() { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa },
            new() { Id = 2, Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita }
        };

        var totaisGerais = new TotaisDto
        {
            TotalReceitas = 5000,
            TotalDespesas = 2000
        };

        _repositoryMock
            .Setup(r => r.GetTotaisPorCategoriaAsync())
            .ReturnsAsync(categorias);

        _repositoryMock
            .Setup(r => r.GetTotaisGeraisAsync())
            .ReturnsAsync(totaisGerais);

        // Act
        var resultado = await _service.ObterTodasAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Categorias.Should().HaveCount(2);
        resultado.TotaisGerais.Should().NotBeNull();
        resultado.TotaisGerais.SaldoLiquido.Should().Be(3000);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarDto()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        _repositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(1))
            .ReturnsAsync(categoria);

        // Act
        var resultado = await _service.ObterPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Descricao.Should().Be("Alimentação");
        resultado.Finalidade.Should().Be(FinalidadeCategoria.Despesa);
    }

    [Fact]
    public async Task CriarAsync_CategoriaValida_DeveCriarERetornarDto()
    {
        // Arrange
        var categoria = new Categoria
        {
            Descricao = "Nova Categoria",
            Finalidade = FinalidadeCategoria.Ambas
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Categoria>()))
            .Returns(Task.CompletedTask)
            .Callback<Categoria>(c => c.Id = 10);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CriarAsync(categoria);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Nova Categoria");
        resultado.Finalidade.Should().Be(FinalidadeCategoria.Ambas);
        
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_CategoriaValida_DeveAtualizar()
    {
        // Arrange
        var categoriaExistente = new Categoria
        {
            Id = 1,
            Descricao = "Categoria Antiga",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var categoriaAtualizada = new Categoria
        {
            Id = 1,
            Descricao = "Categoria Atualizada",
            Finalidade = FinalidadeCategoria.Ambas
        };

        _repositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(1))
            .ReturnsAsync(categoriaExistente);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Categoria>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.AtualizarAsync(categoriaAtualizada);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Categoria Atualizada");
        resultado.Finalidade.Should().Be(FinalidadeCategoria.Ambas);
        
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Categoria>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_CategoriaExiste_DeveDeletar()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Categoria para deletar",
            Finalidade = FinalidadeCategoria.Despesa,
            Transacoes = new List<Transacao>() // Lista vazia, sem transações
        };

        _repositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(1))
            .ReturnsAsync(categoria);

        _repositoryMock
            .Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.DeletarAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
