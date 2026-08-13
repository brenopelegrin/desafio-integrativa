using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciadorProcessos.Domain.Entities;

namespace GerenciadorProcessos.Domain.Interfaces;

public interface IEntidadeLegalRepository
{
    Task<EntidadeLegal?> GetByIdAsync(Guid id);
    Task<EntidadeLegal?> GetByDocumentoAsync(string numeroDocumento);
    Task<(IEnumerable<EntidadeLegal> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? numeroDocumento = null);
    Task AddAsync(EntidadeLegal entidadeLegal);
    Task UpdateAsync(EntidadeLegal entidadeLegal);
}
