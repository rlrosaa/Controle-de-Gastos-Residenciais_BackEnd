using Microsoft.AspNetCore.Mvc;
using ControleGastosResiduais.Application.Services;
using ControleGastosResiduais.Domain.DTOs;
using ControleGastosResiduais.Domain.Exceptions;
using ControleGastosResiduais.Domain.Entities;

namespace ControleGastosResiduais.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de categorias.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaAppService _service;

    public CategoriasController(CategoriaAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna todas as categorias cadastradas com seus totais de receitas, despesas e saldo.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ListaCategoriasDto>> GetAll()
    {
        var resultado = await _service.ObterTodasAsync();
        return Ok(resultado);
    }

    /// <summary>
    /// Retorna uma categoria específica por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        try
        {
            var categoria = await _service.ObterPorIdAsync(id);
            return Ok(categoria);
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova categoria.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Create([FromBody] Categoria categoria)
    {
        try
        {
            var resultado = await _service.CriarAsync(categoria);
            return Ok(resultado);
        }
        catch (ValidacaoNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    [HttpPut("update")]
    public async Task<ActionResult<CategoriaDto>> Update([FromBody] Categoria categoria)
    {
        try
        {
            var resultado = await _service.AtualizarAsync(categoria);
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
    /// Remove uma categoria existente.
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
        catch (ValidacaoNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }
   
}
