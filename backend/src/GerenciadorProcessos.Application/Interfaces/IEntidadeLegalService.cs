using System;
using System.Threading.Tasks;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;

namespace GerenciadorProcessos.Application.Interfaces;

public interface IEntidadeLegalService
{
    Task<PaginatedListDto<EntidadeLegalDto>> GetAllAsync(int pageNumber, int pageSize, string? numeroDocumento = null);
    Task<EntidadeLegalDto?> GetByIdAsync(Guid id);
    Task<EntidadeLegalDto> CreateAsync(CreateEntidadeLegalDto dto);
    Task<EntidadeLegalDto?> UpdateAsync(Guid id, UpdateEntidadeLegalDto dto);
}
