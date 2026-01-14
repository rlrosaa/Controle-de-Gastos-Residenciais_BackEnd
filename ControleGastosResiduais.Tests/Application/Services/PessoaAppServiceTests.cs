using ControleGastosResiduais.Application.Services;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGastosResiduais.Tests.Application.Services;

/// <summary>
/// Testes unitários para PessoaAppService.
/// </summary>
public class PessoaAppServiceTests
{
    private readonly Mock<IPessoaRepository> _repositoryMock;
    private readonly Mock<ILogger<PessoaAppService>> _loggerMock;
    private readonly PessoaAppService _service;

    public PessoaAppServiceTests()
    {
        _repositoryMock = new Mock<IPessoaRepository>();
        _loggerMock = new Mock<ILogger<PessoaAppService>>();
        _service = new PessoaAppService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ObterEntidadePorIdAsync_PessoaExiste_DeveRetornarPessoa()
    {
        // Arrange
        var pessoaEsperada = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pessoaEsperada);

        // Act
        var resultado = await _service.ObterEntidadePorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEquivalentTo(pessoaEsperada);
    }

    [Fact]
    public async Task ObterEntidadePorIdAsync_PessoaNaoExiste_DeveLancarExcecao()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Pessoa?)null);

        // Act
        var act = async () => await _service.ObterEntidadePorIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage("*Pessoa*999*");
    }

    [Fact]
    public async Task CriarAsync_PessoaValida_DeveCriarERetornarDto()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Nome = "Maria Silva",
            Idade = 25
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Pessoa>()))
            .Returns(Task.CompletedTask)
            .Callback<Pessoa>(p => p.Id = 10);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CriarAsync(pessoa);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Maria Silva");
        resultado.Idade.Should().Be(25);
        
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Pessoa>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_PessoaValida_DeveAtualizar()
    {
        // Arrange
        var pessoaExistente = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        var pessoaAtualizada = new Pessoa
        {
            Id = 1,
            Nome = "João Silva Junior",
            Idade = 31
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pessoaExistente);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Pessoa>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.AtualizarAsync(pessoaAtualizada);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("João Silva Junior");
        resultado.Idade.Should().Be(31);
        
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Pessoa>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_PessoaExiste_DeveDeletar()
    {
        // Arrange
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

    [Fact]
    public async Task ObterTotaisAsync_DeveRetornarListaComTotais()
    {
        // Arrange
        var pessoas = new List<PessoaDto>
        {
            new() { Id = 1, Nome = "João Silva", Idade = 30 },
            new() { Id = 2, Nome = "Maria Silva", Idade = 25 }
        };

        var totaisGerais = new TotaisDto
        {
            TotalReceitas = 10000,
            TotalDespesas = 5000
        };

        _repositoryMock
            .Setup(r => r.GetTotaisPorPessoaAsync())
            .ReturnsAsync(pessoas);

        _repositoryMock
            .Setup(r => r.GetTotaisGeraisAsync())
            .ReturnsAsync(totaisGerais);

        // Act
        var resultado = await _service.ObterTotaisAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Pessoas.Should().HaveCount(2);
        resultado.TotaisGerais.Should().NotBeNull();
        resultado.TotaisGerais.SaldoLiquido.Should().Be(5000);
    }
}
