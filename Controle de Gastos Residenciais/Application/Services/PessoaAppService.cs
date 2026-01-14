using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Interfaces;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Exceptions;

namespace ControleGastosResiduais.Application.Services;

/// <summary>
/// Service de aplicação responsável pela orquestração de operações relacionadas a Pessoa.
/// </summary>
public class PessoaAppService
{
    private readonly IPessoaRepository _repository;
    private readonly ILogger<PessoaAppService> _logger;

    public PessoaAppService(IPessoaRepository repository, ILogger<PessoaAppService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obtém a entidade Pessoa por ID, lançando exceção se não encontrada.
    /// Usado internamente por outros serviços que precisam da entidade completa.
    /// </summary>
    public async Task<Pessoa> ObterEntidadePorIdAsync(int id)
    {
        var pessoa = await _repository.GetByIdAsync(id);
        if (pessoa == null)
        {
            throw new EntidadeNaoEncontradaException("Pessoa", id);
        }
        return pessoa;
    }
  

    /// <summary>
    /// Cria uma nova pessoa com validações.
    /// </summary>
    public async Task<PessoaDto> CriarAsync(Pessoa pessoa)
    {
        ValidarPessoa(pessoa);

        await _repository.AddAsync(pessoa);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Pessoa {Id} - {Nome} criada com sucesso", pessoa.Id, pessoa.Nome);
        
        return new PessoaDto
        {
            Id = pessoa.Id.GetValueOrDefault(),
            Nome = pessoa.Nome,
            Idade = pessoa.Idade,
        };
    }

    /// <summary>
    /// Deleta uma pessoa e todas as suas transações (cascade).
    /// </summary>
    public async Task DeletarAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Pessoa {Id} deletada com sucesso", id);
    }

    /// <summary>
    /// Retorna os totais de receitas, despesas e saldo de cada pessoa.
    /// </summary>
    public async Task<ListaPessoasDto> ObterTotaisAsync()
    {
        var totaisPorPessoa = await _repository.GetTotaisPorPessoaAsync();
        var totaisGerais = await _repository.GetTotaisGeraisAsync();

        return new ListaPessoasDto
        {
            Pessoas = totaisPorPessoa,
            TotaisGerais = totaisGerais
        };
    }


    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    public async Task<PessoaDto> AtualizarAsync(Pessoa pessoa)
    {
        ValidarIdObrigatorio(pessoa.Id);
        ValidarPessoa(pessoa);

        var pessoaExistente = await ObterEntidadePorIdAsync(pessoa.Id.GetValueOrDefault());

        pessoaExistente.Nome = pessoa.Nome;
        pessoaExistente.Idade = pessoa.Idade;

        await _repository.UpdateAsync(pessoaExistente);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Pessoa {Id} - {Nome} atualizada com sucesso", pessoaExistente.Id, pessoaExistente.Nome);
        
        return new PessoaDto
        {
            Id = pessoaExistente.Id.GetValueOrDefault(),
            Nome = pessoaExistente.Nome,
            Idade = pessoaExistente.Idade,
        };
    }

    #region Métodos de Validação

    private void ValidarPessoa(Pessoa pessoa)
    {
        if (string.IsNullOrWhiteSpace(pessoa.Nome))
        {
            throw new ValidacaoNegocioException("Nome é obrigatório.");
        }

        if (pessoa.Idade < 0)
        {
            throw new ValidacaoNegocioException("Idade não pode ser negativa.");
        }
    }

    private void ValidarIdObrigatorio(int? id)
    {
        if (id == null || id <= 0)
        {
            throw new ValidacaoNegocioException($"ID da Pessoa é obrigatório para atualização.");
        }
    }

    #endregion
}
