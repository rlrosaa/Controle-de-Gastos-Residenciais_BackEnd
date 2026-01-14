using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Enums;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.Services;
using FluentAssertions;
using Xunit;

namespace ControleGastosResiduais.Tests.Domain.Services;

/// <summary>
/// Testes unitários para o serviço de domínio TransacaoService.
/// </summary>
public class TransacaoServiceTests
{
    private readonly TransacaoService _service;

    public TransacaoServiceTests()
    {
        _service = new TransacaoService();
    }

    [Fact]
    public void ValidarTransacao_CategoriaNaoAceitaTipo_DeveLancarExcecao()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Salário",
            Finalidade = FinalidadeCategoria.Receita
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        var transacao = new Transacao
        {
            Descricao = "Compra de alimentos",
            Valor = 100,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = categoria.Id.Value,
            PessoaId = pessoa.Id.Value
        };

        // Act
        var act = () => _service.ValidarTransacao(transacao, pessoa, categoria);

        // Assert
        act.Should().Throw<ValidacaoNegocioException>()
            .WithMessage("*não permite transações do tipo*");
    }

    [Fact]
    public void ValidarTransacao_MenorDeIdadeCriandoReceita_DeveLancarExcecao()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Mesada",
            Finalidade = FinalidadeCategoria.Receita
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "Maria Silva",
            Idade = 15 // Menor de idade
        };

        var transacao = new Transacao
        {
            Descricao = "Mesada",
            Valor = 50,
            Tipo = TipoTransacao.Receita,
            CategoriaId = categoria.Id.Value,
            PessoaId = pessoa.Id.Value
        };

        // Act
        var act = () => _service.ValidarTransacao(transacao, pessoa, categoria);

        // Assert
        act.Should().Throw<ValidacaoNegocioException>()
            .WithMessage("Menores de idade não podem criar transações de receita.");
    }

    [Fact]
    public void ValidarTransacao_MenorDeIdadeCriandoDespesa_DevePassar()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Lazer",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "Maria Silva",
            Idade = 15 // Menor de idade
        };

        var transacao = new Transacao
        {
            Descricao = "Cinema",
            Valor = 30,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = categoria.Id.Value,
            PessoaId = pessoa.Id.Value
        };

        // Act
        var act = () => _service.ValidarTransacao(transacao, pessoa, categoria);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidarTransacao_MaiorDeIdadeCriandoReceita_DevePassar()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Salário",
            Finalidade = FinalidadeCategoria.Receita
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        var transacao = new Transacao
        {
            Descricao = "Salário Janeiro",
            Valor = 5000,
            Tipo = TipoTransacao.Receita,
            CategoriaId = categoria.Id.Value,
            PessoaId = pessoa.Id.Value
        };

        // Act
        var act = () => _service.ValidarTransacao(transacao, pessoa, categoria);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidarTransacao_CategoriaAmbasComReceita_DevePassar()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Geral",
            Finalidade = FinalidadeCategoria.Ambas
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        var transacao = new Transacao
        {
            Descricao = "Freelance",
            Valor = 1000,
            Tipo = TipoTransacao.Receita,
            CategoriaId = categoria.Id.Value,
            PessoaId = pessoa.Id.Value
        };

        // Act
        var act = () => _service.ValidarTransacao(transacao, pessoa, categoria);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidarTransacao_CategoriaAmbasComDespesa_DevePassar()
    {
        // Arrange
        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Geral",
            Finalidade = FinalidadeCategoria.Ambas
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            Idade = 30
        };

        var transacao = new Transacao
        {
            Descricao = "Diversos",
            Valor = 100,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = categoria.Id.Value,
            PessoaId = pessoa.Id.Value
        };

        // Act
        var act = () => _service.ValidarTransacao(transacao, pessoa, categoria);

        // Assert
        act.Should().NotThrow();
    }
}
