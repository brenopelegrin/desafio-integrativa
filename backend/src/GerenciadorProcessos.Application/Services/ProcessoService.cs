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

public class ProcessoService : IProcessoService
{
    private readonly IProcessoRepository _repository;
    private readonly IEntidadeLegalRepository _entidadeLegalRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessoService(IProcessoRepository repository, IEntidadeLegalRepository entidadeLegalRepository, TimeProvider timeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _entidadeLegalRepository = entidadeLegalRepository;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessoDto?> GetByIdAsync(Guid id)
    {
        var processo = await _repository.GetByIdAsync(id);
        if (processo == null) return null;
        return MapToDto(processo);
    }

    public async Task<PaginatedListDto<ProcessoDto>> GetAllAsync(ProcessoFilterDto filter)
    {
        string? cleanDoc = null;
        if (!string.IsNullOrWhiteSpace(filter.NumeroDocumentoParte))
        {
            cleanDoc = new string(filter.NumeroDocumentoParte.Where(char.IsDigit).ToArray());
        }

        var (items, totalCount) = await _repository.GetAllAsync(filter.PageNumber, filter.PageSize, cleanDoc, filter.StatusProcesso, filter.NumeroProcesso);
        var dtos = items.Select(MapToDto).ToList();
        return new PaginatedListDto<ProcessoDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<ProcessoDto> CreateAsync(CreateProcessoDto dto)
    {
        var partesDomain = new List<Parte>();

        foreach (var p in dto.Partes)
        {
            var entidadeLegal = await ResolveEntidadeLegalAsync(p.EntidadeLegalId, p.NovaEntidadeLegal);
            var parte = new Parte(p.TipoPolo, entidadeLegal);
            partesDomain.Add(parte);
        }

        var processo = new Processo(
            dto.NumeroProcesso,
            dto.TipoProcesso,
            dto.Assunto,
            _timeProvider.GetUtcNow(),
            partesDomain
        );

        await _repository.AddAsync(processo);
        await _unitOfWork.CommitAsync();
        return MapToDto(processo);
    }

    public async Task<ProcessoDto?> UpdateAsync(Guid id, UpdateProcessoDto dto)
    {
        var processo = await _repository.GetByIdAsync(id);
        if (processo == null) return null;

        processo.UpdateAssunto(dto.Assunto);
        processo.ChangeStatus(dto.Status);

        await _repository.UpdateAsync(processo);
        await _unitOfWork.CommitAsync();
        return MapToDto(processo);
    }

    public async Task DeleteAsync(Guid id)
    {
        var processo = await _repository.GetByIdAsync(id);
        if (processo != null)
        {
            processo.Delete();
            await _repository.UpdateAsync(processo);
            await _unitOfWork.CommitAsync();
        }
    }

    public async Task<ProcessoDto?> AddParteAsync(Guid processoId, AddParteDto dto)
    {
        var processo = await _repository.GetByIdAsync(processoId);
        if (processo == null) return null;

        var entidadeLegal = await ResolveEntidadeLegalAsync(dto.EntidadeLegalId, dto.NovaEntidadeLegal);
        var parte = new Parte(dto.TipoPolo, entidadeLegal);
        processo.AddParte(parte);

        await _repository.UpdateAsync(processo);
        await _unitOfWork.CommitAsync();
        return MapToDto(processo);
    }

    public async Task<ProcessoDto?> AddAndamentoAsync(Guid processoId, AddAndamentoDto dto)
    {
        var processo = await _repository.GetByIdAsync(processoId);
        if (processo == null) return null;

        var andamento = new Andamento(dto.DataAndamento, dto.Descricao);
        processo.AddAndamento(andamento, _timeProvider.GetUtcNow());

        await _repository.UpdateAsync(processo);
        await _unitOfWork.CommitAsync();
        return MapToDto(processo);
    }

    public async Task<ProcessoDto?> RemoveParteAsync(Guid processoId, Guid parteId)
    {
        var processo = await _repository.GetByIdAsync(processoId);
        if (processo == null) return null;

        processo.RemoveParte(parteId);
        await _repository.UpdateAsync(processo);
        await _unitOfWork.CommitAsync();
        return MapToDto(processo);
    }

    public async Task<PaginatedListDto<ParteDto>?> GetPartesAsync(Guid processoId, int pageNumber, int pageSize)
    {
        var processo = await _repository.GetByIdAsync(processoId);
        if (processo == null) return null;

        var (items, totalCount) = await _repository.GetPartesAsync(processoId, pageNumber, pageSize);
        var dtos = items.Select(MapParteToDto).ToList();
        return new PaginatedListDto<ParteDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedListDto<AndamentoDto>?> GetAndamentosAsync(Guid processoId, int pageNumber, int pageSize)
    {
        var processo = await _repository.GetByIdAsync(processoId);
        if (processo == null) return null;

        var (items, totalCount) = await _repository.GetAndamentosAsync(processoId, pageNumber, pageSize);
        var dtos = items.Select(a => new AndamentoDto(a.Id, a.Data, a.Descricao)).ToList();
        return new PaginatedListDto<AndamentoDto>(dtos, totalCount, pageNumber, pageSize);
    }

    private async Task<EntidadeLegal> ResolveEntidadeLegalAsync(Guid? id, CreateEntidadeLegalDto? novaEntidade)
    {
        if (id.HasValue && id.Value != Guid.Empty)
        {
            var entity = await _entidadeLegalRepository.GetByIdAsync(id.Value);
            if (entity == null)
                throw new DomainException("Entidade Legal informada não existe.");
            return entity;
        }

        if (novaEntidade != null)
        {
            var cleanDoc = new string(novaEntidade.NumeroDocumento.Where(char.IsDigit).ToArray());
            var exists = await _entidadeLegalRepository.GetByDocumentoAsync(cleanDoc);
            if (exists != null)
                throw new DomainException($"Já existe uma Entidade Legal com o documento {cleanDoc}. Utilize o ID existente ou atualize-a via endpoint próprio.");

            var entidade = new EntidadeLegal(novaEntidade.Nome, novaEntidade.TipoEntidade, novaEntidade.NumeroDocumento);
            await _entidadeLegalRepository.AddAsync(entidade);
            return entidade;
        }

        throw new DomainException("É necessário informar o Id da Entidade Legal ou os dados para criar uma nova.");
    }

    private static ProcessoDto MapToDto(Processo processo)
    {
        return new ProcessoDto(
            processo.Id,
            processo.NumeroProcesso,
            processo.TipoProcesso,
            processo.Assunto,
            processo.DataCriacao,
            processo.Status,
            processo.Partes.Where(p => !p.FlagDeleted).Select(MapParteToDto).ToList(),
            processo.Andamentos.Where(a => !a.FlagDeleted).OrderByDescending(a => a.Data).Select(a => new AndamentoDto(a.Id, a.Data, a.Descricao)).ToList()
        );
    }

    private static ParteDto MapParteToDto(Parte p)
    {
        return new ParteDto(
            p.Id,
            p.TipoPolo,
            EntidadeLegalService.MapToDto(p.EntidadeLegal)
        );
    }
}
