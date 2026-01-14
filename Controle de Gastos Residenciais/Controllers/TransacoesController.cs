using Microsoft.AspNetCore.Mvc;
using ControleGastosResiduais.Application.Services;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Entities;
using ControleGastosResiduais.Domain.Exceptions;

namespace ControleGastosResiduais.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de transações.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly TransacaoAppService _service;

    public TransacoesController(TransacaoAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoDto>>> GetAll()
    {
        var transacoes = await _service.ObterTodasAsync();
        return Ok(transacoes);
    }

    /// <summary>
    /// Retorna uma transação específica por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransacaoDto>> GetById(int id)
    {
        try
        {
            var transacao = await _service.ObterPorIdAsync(id);
            return Ok(transacao);
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova transação com validação de regras de negócio.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransacaoDto>> Create([FromBody] Transacao transacao)
    {
        try
        {
            var resultado = await _service.CriarAsync(transacao);
            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
        }
        catch (ValidacaoNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    [HttpPut("update")]
    public async Task<ActionResult<TransacaoDto>> Update([FromBody] Transacao transacao)
    {
        try
        {
            var resultado = await _service.AtualizarAsync(transacao);
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
    /// Remove uma transação existente.
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
