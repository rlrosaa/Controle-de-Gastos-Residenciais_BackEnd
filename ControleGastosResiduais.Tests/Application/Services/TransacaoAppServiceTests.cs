using ControleGastosResiduais.Application.Services;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ControleGastosResiduais.Tests.Application.Services;

/// <summary>
/// Testes unitários para TransacaoAppService.
/// </summary>
public class TransacaoAppServiceTests
{
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
    private readonly Mock<IPessoaRepository> _pessoaRepositoryMock;
    private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
    private readonly PessoaAppService _pessoaAppService;
    private readonly CategoriaAppService _categoriaAppService;
    private readonly TransacaoService _transacaoService;
    private readonly Mock<ILogger<TransacaoAppService>> _loggerMock;
    private readonly TransacaoAppService _service;

    public TransacaoAppServiceTests()
    {
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _pessoaRepositoryMock = new Mock<IPessoaRepository>();
        _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
        
        var pessoaLoggerMock = new Mock<ILogger<PessoaAppService>>();
        _pessoaAppService = new PessoaAppService(_pessoaRepositoryMock.Object, pessoaLoggerMock.Object);
        
        var categoriaLoggerMock = new Mock<ILogger<CategoriaAppService>>();
        _categoriaAppService = new CategoriaAppService(_categoriaRepositoryMock.Object, categoriaLoggerMock.Object);
        
        _transacaoService = new TransacaoService();
        _loggerMock = new Mock<ILogger<TransacaoAppService>>();

        _service = new TransacaoAppService(
            _transacaoRepositoryMock.Object,
            _pessoaAppService,
            _categoriaAppService,
            _transacaoService,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRetornarListaDeTransacoes()
    {
        // Arrange
        var transacoes = new List<Transacao>
        {
            new()
            {
                Id = 1,
                Descricao = "Compra supermercado",
                Valor = 150,
                Tipo = TipoTransacao.Despesa,
                CategoriaId = 1,
                Categoria = new Categoria { Id = 1, Descricao = "Alimentação" },
                PessoaId = 1,
                Pessoa = new Pessoa { Id = 1, Nome = "João Silva" }
            },
            new()
            {
                Id = 2,
                Descricao = "Salário",
                Valor = 5000,
                Tipo = TipoTransacao.Receita,
                CategoriaId = 2,
                Categoria = new Categoria { Id = 2, Descricao = "Salário" },
                PessoaId = 1,
                Pessoa = new Pessoa { Id = 1, Nome = "João Silva" }
            }
        };

        _transacaoRepositoryMock
            .Setup(r => r.GetAllWithRelationsAsync())
            .ReturnsAsync(transacoes);

        // Act
        var resultado = await _service.ObterTodasAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Descricao.Should().Be("Compra supermercado");
    }

    [Fact]
    public async Task ObterPorIdAsync_TransacaoExiste_DeveRetornarDto()
    {
        // Arrange
        var transacao = new Transacao
        {
            Id = 1,
            Descricao = "Compra supermercado",
            Valor = 150,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            Categoria = new Categoria { Id = 1, Descricao = "Alimentação" },
            PessoaId = 1,
            Pessoa = new Pessoa { Id = 1, Nome = "João Silva" }
        };

        _transacaoRepositoryMock
            .Setup(r => r.GetByIdWithRelationsAsync(1))
            .ReturnsAsync(transacao);

        // Act
        var resultado = await _service.ObterPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Descricao.Should().Be("Compra supermercado");
        resultado.Valor.Should().Be(150);
        resultado.CategoriaNome.Should().Be("Alimentação");
        resultado.PessoaNome.Should().Be("João Silva");
    }

    [Fact]
    public async Task ObterPorIdAsync_TransacaoNaoExiste_DeveLancarExcecao()
    {
        // Arrange
        _transacaoRepositoryMock
            .Setup(r => r.GetByIdWithRelationsAsync(999))
            .ReturnsAsync((Transacao?)null);

        // Act
        var act = async () => await _service.ObterPorIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage("*Transação*999*");
    }

    [Fact]
    public async Task CriarAsync_TransacaoValida_DeveCriarERetornarDto()
    {
        // Arrange
        var pessoa = new Pessoa { Id = 1, Nome = "João Silva", Idade = 30 };
        var categoria = new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa, Transacoes = new List<Transacao>() };
        
        var transacao = new Transacao
        {
            Descricao = "Compra supermercado",
            Valor = 150,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        _pessoaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(1))
            .ReturnsAsync(categoria);

        _transacaoRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Transacao>()))
            .Returns(Task.CompletedTask)
            .Callback<Transacao>(t => t.Id = 10);

        _transacaoRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.CriarAsync(transacao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Compra supermercado");
        resultado.Valor.Should().Be(150);
        resultado.CategoriaNome.Should().Be("Alimentação");
        resultado.PessoaNome.Should().Be("João Silva");
        
        _transacaoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transacao>()), Times.Once);
        _transacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DescricaoVazia_DeveLancarExcecao()
    {
        // Arrange
        var transacao = new Transacao
        {
            Descricao = "",
            Valor = 150,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        // Act
        var act = async () => await _service.CriarAsync(transacao);

        // Assert
        await act.Should().ThrowAsync<ValidacaoNegocioException>()
            .WithMessage("*Descrição é obrigatória*");
    }

    [Fact]
    public async Task CriarAsync_ValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var transacao = new Transacao
        {
            Descricao = "Teste",
            Valor = -100,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        // Act
        var act = async () => await _service.CriarAsync(transacao);

        // Assert
        await act.Should().ThrowAsync<ValidacaoNegocioException>()
            .WithMessage("*Valor deve ser positivo*");
    }

    [Fact]
    public async Task AtualizarAsync_TransacaoValida_DeveAtualizar()
    {
        // Arrange
        var pessoa = new Pessoa { Id = 1, Nome = "João Silva", Idade = 30 };
        var categoria = new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa, Transacoes = new List<Transacao>() };
        
        var transacaoExistente = new Transacao
        {
            Id = 1,
            Descricao = "Transação Antiga",
            Valor = 100,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        var transacaoAtualizada = new Transacao
        {
            Id = 1,
            Descricao = "Transação Atualizada",
            Valor = 200,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        _transacaoRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(transacaoExistente);

        _pessoaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(r => r.GetByIdComTransacoesAsync(1))
            .ReturnsAsync(categoria);

        _transacaoRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Transacao>()))
            .Returns(Task.CompletedTask);

        _transacaoRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _service.AtualizarAsync(transacaoAtualizada);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Transação Atualizada");
        resultado.Valor.Should().Be(200);
        
        _transacaoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Transacao>()), Times.Once);
        _transacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_TransacaoExiste_DeveDeletar()
    {
        // Arrange
        var transacao = new Transacao
        {
            Id = 1,
            Descricao = "Transação para deletar",
            Valor = 100,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = 1,
            PessoaId = 1
        };

        _transacaoRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(transacao);

        _transacaoRepositoryMock
            .Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        _transacaoRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.DeletarAsync(1);

        // Assert
        _transacaoRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        _transacaoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_TransacaoNaoExiste_DeveLancarExcecao()
    {
        // Arrange
        _transacaoRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Transacao?)null);

        // Act
        var act = async () => await _service.DeletarAsync(999);

        // Assert
        await act.Should().ThrowAsync<EntidadeNaoEncontradaException>()
            .WithMessage("*Transação*999*");
    }
}
