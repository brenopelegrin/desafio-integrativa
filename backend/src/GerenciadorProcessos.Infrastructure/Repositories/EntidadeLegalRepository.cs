using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciadorProcessos.Domain.Entities;
using GerenciadorProcessos.Domain.Interfaces;
using GerenciadorProcessos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorProcessos.Infrastructure.Repositories;

public class EntidadeLegalRepository : IEntidadeLegalRepository
{
    private readonly AppDbContext _context;

    public EntidadeLegalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EntidadeLegal?> GetByIdAsync(Guid id)
    {
        return await _context.EntidadesLegais.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<EntidadeLegal?> GetByDocumentoAsync(string numeroDocumento)
    {
        return await _context.EntidadesLegais.FirstOrDefaultAsync(e => e.NumeroDocumento == numeroDocumento);
    }

    public async Task<(IEnumerable<EntidadeLegal> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? numeroDocumento = null)
    {
        var query = _context.EntidadesLegais.AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(numeroDocumento))
        {
            query = query.Where(e => e.NumeroDocumento == numeroDocumento);
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(e => e.Nome)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (items, totalCount);
    }

    public async Task AddAsync(EntidadeLegal entidadeLegal)
    {
        await _context.EntidadesLegais.AddAsync(entidadeLegal);
    }

    public Task UpdateAsync(EntidadeLegal entidadeLegal)
    {
        _context.EntidadesLegais.Update(entidadeLegal);
        return Task.CompletedTask;
    }
}
