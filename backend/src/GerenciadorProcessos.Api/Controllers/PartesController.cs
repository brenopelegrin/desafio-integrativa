using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;
using GerenciadorProcessos.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorProcessos.Api.Controllers;

[Route("api/v1/processos/{processoId:guid}/[controller]")]
public class PartesController : ApiControllerBase
{
    private readonly IProcessoService _processoService;

    public PartesController(IProcessoService processoService)
    {
        _processoService = processoService;
    }

    /// <summary>
    /// Lista todas as Partes associadas a um Processo, com paginação
    /// </summary>
    /// <param name="processoId">O ID do processo</param>
    /// <param name="pageNumber">O número da página atual (padrão: 1)</param>
    /// <param name="pageSize">O tamanho da página (padrão: 10, máximo: 50)</param>
    /// <returns>Uma lista paginada de partes do processo</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedListDto<ParteDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedListDto<ParteDto>>> GetAll(
        Guid processoId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Validação dos parâmetros de paginação
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 50 ? 50 : pageSize);

        var result = await _processoService.GetPartesAsync(processoId, pageNumber, pageSize);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Adiciona uma nova Parte a um Processo existente.
    /// </summary>
    /// <param name="processoId">O ID do Processo</param>
    /// <param name="dto">Os dados da Parte a ser adicionada</param>
    /// <returns>O Processo atualizado com a nova parte</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessoDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessoDto>> AddParte(Guid processoId, AddParteDto dto)
    {
        var result = await _processoService.AddParteAsync(processoId, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Remove uma Parte de um Processo (Apenas soft delete).
    /// </summary>
    /// <param name="processoId">O ID do Processo</param>
    /// <param name="parteId">O ID da Parte a ser removida</param>
    /// <returns>O Processo atualizado sem a Parte</returns>
    [HttpDelete("{parteId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessoDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessoDto>> RemoveParte(Guid processoId, Guid parteId)
    {
        var result = await _processoService.RemoveParteAsync(processoId, parteId);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
