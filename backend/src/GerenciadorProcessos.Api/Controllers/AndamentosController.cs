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
public class AndamentosController : ApiControllerBase
{
    private readonly IProcessoService _processoService;

    public AndamentosController(IProcessoService processoService)
    {
        _processoService = processoService;
    }

    /// <summary>
    /// Lista todos os Andamentos de um Processo, com paginação
    /// </summary>
    /// <param name="processoId">O ID do processo</param>
    /// <param name="pageNumber">O número da página atual (padrão: 1)</param>
    /// <param name="pageSize">O tamanho da página (padrão: 10, máximo: 50)</param>
    /// <returns>Uma lista paginada de Andamentos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedListDto<AndamentoDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedListDto<AndamentoDto>>> GetAll(
        Guid processoId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Validação dos parâmetros de paginação
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 50 ? 50 : pageSize);

        var result = await _processoService.GetAndamentosAsync(processoId, pageNumber, pageSize);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Adiciona um novo Andamento a um Processo existente.
    /// </summary>
    /// <param name="processoId">O ID do processo</param>
    /// <param name="dto">Os dados do Andamento a ser adicionado</param>
    /// <returns>O Processo atualizado com o novo Andamento</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessoDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessoDto>> AddAndamento(Guid processoId, AddAndamentoDto dto)
    {
        var result = await _processoService.AddAndamentoAsync(processoId, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
