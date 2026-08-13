using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciadorProcessos.Domain.Entities;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Domain.Interfaces;

public interface IProcessoRepository
{
    Task<Processo?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Processo> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? numeroDocumentoParte = null, StatusProcesso? statusProcesso = null, string? numeroProcesso = null);
    Task AddAsync(Processo processo);
    Task UpdateAsync(Processo processo);
    Task DeleteAsync(Processo processo);
    
    Task<(IEnumerable<Parte> Items, int TotalCount)> GetPartesAsync(Guid processoId, int pageNumber, int pageSize);
    Task<(IEnumerable<Andamento> Items, int TotalCount)> GetAndamentosAsync(Guid processoId, int pageNumber, int pageSize);
}
