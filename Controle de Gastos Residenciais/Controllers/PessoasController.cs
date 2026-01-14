using Microsoft.AspNetCore.Mvc;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Application.Services;
using ControleGastosResiduais.Domain.Exceptions;

namespace ControleGastosResiduais.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de pessoas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly PessoaAppService _service;

    public PessoasController(PessoaAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna todas as pessoas cadastradas com seus totais de receitas, despesas e saldo.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ListaPessoasDto>> GetAll()
    {
        var resultado = await _service.ObterTotaisAsync();
        return Ok(resultado);
    }

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PessoaDto>> Create([FromBody] Pessoa pessoa)
    {
        try
        {
            var resultado = await _service.CriarAsync(pessoa);
            return Ok(resultado);
        }
        catch (ValidacaoNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    [HttpPut("update")]
    public async Task<ActionResult<PessoaDto>> Update([FromBody] Pessoa pessoa)
    {
        try
        {
            var resultado = await _service.AtualizarAsync(pessoa);
            return Ok(resultado);
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidacaoNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deleta uma pessoa e todas as suas transações (cascade).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _service.DeletarAsync(id);
            return NoContent();
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
