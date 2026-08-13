using System;
using System.Threading.Tasks;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.Interfaces;

public interface IProcessoService
{
    Task<ProcessoDto?> GetByIdAsync(Guid id);
    Task<PaginatedListDto<ProcessoDto>> GetAllAsync(ProcessoFilterDto filter);
    Task<ProcessoDto> CreateAsync(CreateProcessoDto dto);
    Task<ProcessoDto?> UpdateAsync(Guid id, UpdateProcessoDto dto);
    Task DeleteAsync(Guid id);
    Task<ProcessoDto?> AddParteAsync(Guid processoId, AddParteDto dto);
    Task<ProcessoDto?> AddAndamentoAsync(Guid processoId, AddAndamentoDto dto);
    Task<ProcessoDto?> RemoveParteAsync(Guid processoId, Guid parteId);

    // GETs para os "subcontrollers" de Partes e Andamentos
    Task<PaginatedListDto<ParteDto>?> GetPartesAsync(Guid processoId, int pageNumber, int pageSize);
    Task<PaginatedListDto<AndamentoDto>?> GetAndamentosAsync(Guid processoId, int pageNumber, int pageSize);
}
