using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.Services;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Enums;

namespace ControleGastosResiduais.Application.Services;

/// <summary>
/// Service de aplicação responsável pela orquestração de operações relacionadas a Transacao.
/// Coordena validações de negócio e interações entre múltiplos serviços.
/// </summary>
public class TransacaoAppService
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly PessoaAppService _pessoaAppService;
    private readonly CategoriaAppService _categoriaAppService;
    private readonly TransacaoService _transacaoService;
    private readonly ILogger<TransacaoAppService> _logger;

    public TransacaoAppService(
        ITransacaoRepository transacaoRepository,
        PessoaAppService pessoaAppService,
        CategoriaAppService categoriaAppService,
        TransacaoService transacaoService,
        ILogger<TransacaoAppService> logger)
    {
        _transacaoRepository = transacaoRepository;
        _pessoaAppService = pessoaAppService;
        _categoriaAppService = categoriaAppService;
        _transacaoService = transacaoService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    public async Task<IEnumerable<TransacaoDto>> ObterTodasAsync()
    {
        var transacoes = await _transacaoRepository.GetAllWithRelationsAsync();
        return transacoes.Select(t => new TransacaoDto
        {
            Id = t.Id.GetValueOrDefault(),
            Descricao = t.Descricao,
            Valor = t.Valor,
            Tipo = t.Tipo,
            CategoriaId = t.CategoriaId,
            CategoriaNome = t.Categoria.Descricao,
            PessoaId = t.PessoaId,
            PessoaNome = t.Pessoa.Nome,
        });
    }

    /// <summary>
    /// Busca uma transação por ID.
    /// </summary>
    public async Task<TransacaoDto> ObterPorIdAsync(int id)
    {
        var transacao = await _transacaoRepository.GetByIdWithRelationsAsync(id);
        if (transacao == null)
        {
            throw new EntidadeNaoEncontradaException("Transação", id);
        }

        return new TransacaoDto
        {
            Id = transacao.Id.GetValueOrDefault(),
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            CategoriaId = transacao.CategoriaId,
            CategoriaNome = transacao.Categoria.Descricao,
            PessoaId = transacao.PessoaId,
            PessoaNome = transacao.Pessoa.Nome,
        };
    }

    /// <summary>
    /// Cria uma nova transação com validação completa de regras de negócio.
    /// </summary>
    public async Task<TransacaoDto> CriarAsync(Transacao transacao)
    {
        ValidarTransacaoBasica(transacao);

        // Busca entidades relacionadas usando AppServices
        var pessoa = await _pessoaAppService.ObterEntidadePorIdAsync(transacao.PessoaId);
        var categoria = await _categoriaAppService.ObterEntidadePorIdAsync(transacao.CategoriaId);

        // Valida regras de negócio
        _transacaoService.ValidarTransacao(transacao, pessoa, categoria);

        // Persiste
        await _transacaoRepository.AddAsync(transacao);
        await _transacaoRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Transação {Id} criada - {Tipo}: {Valor:C} para {Pessoa}",
            transacao.Id, transacao.Tipo, transacao.Valor, pessoa.Nome);

        return new TransacaoDto
        {
            Id = transacao.Id.GetValueOrDefault(),
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            CategoriaId = transacao.CategoriaId,
            CategoriaNome = categoria.Descricao,
            PessoaId = transacao.PessoaId,
            PessoaNome = pessoa.Nome
        };
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    public async Task<TransacaoDto> AtualizarAsync(Transacao transacao)
    {
        ValidarIdObrigatorio(transacao.Id);
        ValidarTransacaoBasica(transacao);

        var transacaoExistente = await _transacaoRepository.GetByIdAsync(transacao.Id.GetValueOrDefault());
        if (transacaoExistente == null)
        {
            throw new EntidadeNaoEncontradaException("Transação", transacao.Id.GetValueOrDefault());
        }

        // Busca entidades relacionadas usando AppServices
        var pessoa = await _pessoaAppService.ObterEntidadePorIdAsync(transacao.PessoaId);
        var categoria = await _categoriaAppService.ObterEntidadePorIdAsync(transacao.CategoriaId);

        // Atualiza dados
        transacaoExistente.Descricao = transacao.Descricao;
        transacaoExistente.Valor = transacao.Valor;
        transacaoExistente.Tipo = transacao.Tipo;
        transacaoExistente.CategoriaId = transacao.CategoriaId;
        transacaoExistente.PessoaId = transacao.PessoaId;

        _transacaoService.ValidarTransacao(transacaoExistente, pessoa, categoria);

        await _transacaoRepository.UpdateAsync(transacaoExistente);
        await _transacaoRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Transação {Id} atualizada - {Tipo}: {Valor:C} para {Pessoa}",
            transacaoExistente.Id, transacaoExistente.Tipo, transacaoExistente.Valor, pessoa.Nome);

        return new TransacaoDto
        {
            Id = transacaoExistente.Id.GetValueOrDefault(),
            Descricao = transacaoExistente.Descricao,
            Valor = transacaoExistente.Valor,
            Tipo = transacaoExistente.Tipo,
            CategoriaId = transacaoExistente.CategoriaId,
            CategoriaNome = categoria.Descricao,
            PessoaId = transacaoExistente.PessoaId,
            PessoaNome = pessoa.Nome
        };
    }

    /// <summary>
    /// Remove uma transação existente.
    /// </summary>
    public async Task DeletarAsync(int id)
    {
        var transacao = await _transacaoRepository.GetByIdAsync(id);
        if (transacao == null)
        {
            throw new EntidadeNaoEncontradaException("Transação", id);
        }

        await _transacaoRepository.DeleteAsync(id);
        await _transacaoRepository.SaveChangesAsync();

        _logger.LogInformation("Transação {Id} deletada com sucesso", id);
    }

    #region Métodos de Validação

    private static void ValidarTransacaoBasica(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
        {
            throw new ValidacaoNegocioException("Descrição é obrigatória.");
        }

        if (transacao.Valor <= 0)
        {
            throw new ValidacaoNegocioException("Valor deve ser positivo.");
        }

        if (!Enum.IsDefined(transacao.Tipo))
        {
            throw new ValidacaoNegocioException("Tipo de transação inválido. Valores permitidos: 0 (Despesa), 1 (Receita).");
        }
    }

    private static void ValidarIdObrigatorio(int? id)
    {
        if (id == null || id <= 0)
        {
            throw new ValidacaoNegocioException($"ID da transação é obrigatório para atualização.");
        }
    }

    #endregion
}
