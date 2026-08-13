using System;
using System.Threading.Tasks;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;
using GerenciadorProcessos.Application.Interfaces;
using GerenciadorProcessos.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorProcessos.Api.Controllers;

public class ProcessosController : ApiControllerBase
{
    private readonly IProcessoService _processoService;

    public ProcessosController(IProcessoService processoService)
    {
        _processoService = processoService;
    }

    /// <summary>
    /// Lista todos os Processos, com paginação.
    /// </summary>
    /// <param name="pageNumber">O número da página (padrão: 1)</param>
    /// <param name="pageSize">O tamanho da página (padrão: 10)</param>
    /// <param name="numeroDocumentoParte">Filtra os Processos por documento de uma das partes envolvidas</param>
    /// <param name="statusProcesso">Filtra os Processos por um status específico (Ativo, Finalizado, Arquivado)</param>
    /// <param name="numeroProcesso">Filtra os Processos por número (pode ser parcial ou total)</param>
    /// <returns>Uma lista paginada de Processos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedListDto<ProcessoDto>))]
    public async Task<ActionResult<PaginatedListDto<ProcessoDto>>> GetAll([FromQuery] ProcessoFilterDto filter)
    {
        var result = await _processoService.GetAllAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Retorna as informações de um Processo pelo seu ID.
    /// </summary>
    /// <param name="id">O ID do Processo</param>
    /// <returns>Os detalhes do Processo</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessoDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessoDto>> GetById(Guid id)
    {
        var result = await _processoService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo Processo.
    /// </summary>
    /// <param name="dto">Os dados do novo Processo</param>
    /// <returns>O Processo criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProcessoDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProcessoDto>> Create(CreateProcessoDto dto)
    {
        var result = await _processoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza os dados de um Processo existente.
    /// </summary>
    /// <param name="id">O ID do Processo a ser atualizado</param>
    /// <param name="dto">Os novos dados do Processo</param>
    /// <returns>O Processo atualizado</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessoDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessoDto>> Update(Guid id, UpdateProcessoDto dto)
    {
        var result = await _processoService.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Exclui um Processo (Apenas soft delete).
    /// </summary>
    /// <param name="id">O ID do Processo a ser excluído</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _processoService.DeleteAsync(id);
        return NoContent();
    }

}
