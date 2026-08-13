using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciadorProcessos.Application.DTOs.Requests;
using GerenciadorProcessos.Application.DTOs.Responses;
using GerenciadorProcessos.Application.Interfaces;
using GerenciadorProcessos.Domain.Entities;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Domain.Exceptions;
using GerenciadorProcessos.Domain.Interfaces;

namespace GerenciadorProcessos.Application.Services;

public class EntidadeLegalService : IEntidadeLegalService
{
    private readonly IEntidadeLegalRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public EntidadeLegalService(IEntidadeLegalRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedListDto<EntidadeLegalDto>> GetAllAsync(int pageNumber, int pageSize, string? numeroDocumento = null)
    {
        string? cleanDoc = null;
        if (!string.IsNullOrWhiteSpace(numeroDocumento))
        {
            cleanDoc = new string(numeroDocumento.Where(char.IsDigit).ToArray());
        }

        var (items, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, cleanDoc);
        var dtos = items.Select(MapToDto).ToList();
        return new PaginatedListDto<EntidadeLegalDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<EntidadeLegalDto?> GetByIdAsync(Guid id)
    {
        var entidade = await _repository.GetByIdAsync(id);
        if (entidade == null) return null;
        return MapToDto(entidade);
    }

    public async Task<EntidadeLegalDto> CreateAsync(CreateEntidadeLegalDto dto)
    {
        var cleanDoc = new string(dto.NumeroDocumento.Where(char.IsDigit).ToArray());
        var exists = await _repository.GetByDocumentoAsync(cleanDoc);
        if (exists != null)
            throw new DomainException("Já existe uma Entidade Legal com este número de documento.");

        var entidade = new EntidadeLegal(dto.Nome, dto.TipoEntidade, dto.NumeroDocumento);
        await _repository.AddAsync(entidade);
        await _unitOfWork.CommitAsync();
        return MapToDto(entidade);
    }

    public async Task<EntidadeLegalDto?> UpdateAsync(Guid id, UpdateEntidadeLegalDto dto)
    {
        var entidade = await _repository.GetByIdAsync(id);
        if (entidade == null) return null;

        entidade.UpdateNome(dto.Nome);
        await _repository.UpdateAsync(entidade);
        await _unitOfWork.CommitAsync();
        return MapToDto(entidade);
    }

    internal static EntidadeLegalDto MapToDto(EntidadeLegal entidade)
    {
        return new EntidadeLegalDto(
            entidade.Id,
            entidade.Nome,
            entidade.TipoEntidade,
            entidade.NumeroDocumento
        );
    }
}
