using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.Enums;

namespace ControleGastosResiduais.Application.Services;

/// <summary>
/// Service de aplicação responsável pela orquestração de operações relacionadas a Categoria.
/// </summary>
public class CategoriaAppService
{
    private readonly ICategoriaRepository _repository;
    private readonly ILogger<CategoriaAppService> _logger;

    public CategoriaAppService(ICategoriaRepository repository, ILogger<CategoriaAppService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtém a entidade Categoria por ID, lançando exceção se não encontrada.
    /// Usado internamente por outros serviços que precisam da entidade completa.
    /// </summary>
    public async Task<Categoria> ObterEntidadePorIdAsync(int id)
    {
        var categoria = await _repository.GetByIdComTransacoesAsync(id);
        if (categoria == null)
        {
            throw new EntidadeNaoEncontradaException("Categoria", id);
        }
        return categoria;
    }

    /// <summary>
    /// Retorna todas as categorias cadastradas com seus totais de receitas, despesas e saldo.
    /// </summary>
    public async Task<ListaCategoriasDto> ObterTodasAsync()
    {
        var totaisPorCategoria = await _repository.GetTotaisPorCategoriaAsync();
        var totaisGerais = await _repository.GetTotaisGeraisAsync();

        return new ListaCategoriasDto
        {
            Categorias = totaisPorCategoria,
            TotaisGerais = totaisGerais
        };
    }

    /// <summary>
    /// Busca categoria por ID e retorna DTO para uso externo.
    /// </summary>
    public async Task<CategoriaDto> ObterPorIdAsync(int id)
    {
        var categoria = await ObterEntidadePorIdAsync(id);

        return new CategoriaDto
        {
            Id = categoria.Id.GetValueOrDefault(),
            Descricao = categoria.Descricao,
            Finalidade = categoria.Finalidade
        };
    }

    /// <summary>
    /// Cria uma nova categoria com validações.
    /// </summary>
    public async Task<CategoriaDto> CriarAsync(Categoria categoria)
    {
        ValidarCategoria(categoria);

        await _repository.AddAsync(categoria);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Categoria {Id} - {Descricao} criada com sucesso", categoria.Id, categoria.Descricao);

        return new CategoriaDto
        {
            Id = categoria.Id.GetValueOrDefault(),
            Descricao = categoria.Descricao,
            Finalidade = categoria.Finalidade
        };
    }

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    public async Task<CategoriaDto> AtualizarAsync(Categoria categoria)
    {
        ValidarIdObrigatorio(categoria.Id);
        ValidarCategoria(categoria);

        var categoriaExistente = await ObterEntidadePorIdAsync(categoria.Id.GetValueOrDefault());

        categoriaExistente.Descricao = categoria.Descricao;
        categoriaExistente.Finalidade = categoria.Finalidade;

        await _repository.UpdateAsync(categoriaExistente);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Categoria {Id} - {Descricao} atualizada com sucesso", categoriaExistente.Id, categoriaExistente.Descricao);

        return new CategoriaDto
        {
            Id = categoriaExistente.Id.GetValueOrDefault(),
            Descricao = categoriaExistente.Descricao,
            Finalidade = categoriaExistente.Finalidade
        };
    }

    /// <summary>
    /// Remove uma categoria existente.
    /// </summary>
    public async Task DeletarAsync(int id)
    {
        var categoria = await ObterEntidadePorIdAsync(id);

        ValidarExclusaoCategoria(categoria);

        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Categoria {Id} - {Descricao} deletada com sucesso", id, categoria.Descricao);
    }

    #region Métodos de Validação

    private static void ValidarCategoria(Categoria categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Descricao))
        {
            throw new ValidacaoNegocioException("Descrição é obrigatória.");
        }

        if (!Enum.IsDefined(categoria.Finalidade))
        {
            throw new ValidacaoNegocioException("Finalidade inválida. Valores permitidos: 0 (Despesa), 1 (Receita), 2 (Ambas).");
        }
    }

    private static void ValidarIdObrigatorio(int? id)
    {
        if (id == null || id <= 0)
        {
            throw new ValidacaoNegocioException($"ID da Categoria é obrigatório para atualização.");
        }
    }

    private static void ValidarExclusaoCategoria(Categoria categoria)
    {
        if (categoria.Transacoes != null && categoria.Transacoes.Any())
        {
            throw new ValidacaoNegocioException(
                $"Não é possível excluir a categoria '{categoria.Descricao}' pois existem {categoria.Transacoes.Count} transação(ões) associada(s) a ela.");
        }
    }

    #endregion
}
        
