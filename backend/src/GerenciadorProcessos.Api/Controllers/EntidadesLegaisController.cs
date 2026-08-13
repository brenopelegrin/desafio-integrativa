using System;
using System.Threading.Tasks;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;
using GerenciadorProcessos.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorProcessos.Api.Controllers;

[Route("api/v1/entidades-legais")]
public class EntidadesLegaisController : ApiControllerBase
{
    private readonly IEntidadeLegalService _service;

    public EntidadesLegaisController(IEntidadeLegalService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todas as Entidades Legais, com paginação.
    /// </summary>
    /// <param name="pageNumber">O número da página atual (padrão: 1)</param>
    /// <param name="pageSize">O tamanho da página (padrão: 10, máximo: 50)</param>
    /// <param name="numeroDocumento">O número do documento sem formatação (CPF/CNPJ) para filtragem</param>
    /// <returns>Uma lista paginada de Entidades Legais</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedListDto<EntidadeLegalDto>))]
    public async Task<ActionResult<PaginatedListDto<EntidadeLegalDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? numeroDocumento = null)
    {
        // Validação dos parâmetros de paginação
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 50 ? 50 : pageSize);

        var result = await _service.GetAllAsync(pageNumber, pageSize, numeroDocumento);
        return Ok(result);
    }

    /// <summary>
    /// Busca uma Entidade Legal pelo ID.
    /// </summary>
    /// <param name="id">O ID da Entidade Legal</param>
    /// <returns>A Entidade Legal que possui o ID informado</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EntidadeLegalDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntidadeLegalDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova Entidade Legal no sistema.
    /// </summary>
    /// <param name="dto">Os dados da nova Entidade Legal</param>
    /// <returns>A Entidade Legal criada</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EntidadeLegalDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntidadeLegalDto>> Create(CreateEntidadeLegalDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza os dados de uma Entidade Legal existente.
    /// </summary>
    /// <param name="id">O ID da Entidade Legal</param>
    /// <param name="dto">Os dados a serem atualizados</param>
    /// <returns>A Entidade Legal atualizada</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EntidadeLegalDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntidadeLegalDto>> Update(Guid id, UpdateEntidadeLegalDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
