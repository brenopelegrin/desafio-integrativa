using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciadorProcessos.Domain.Entities;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Domain.Interfaces;
using GerenciadorProcessos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorProcessos.Infrastructure.Repositories;

public class ProcessoRepository : IProcessoRepository
{
    private readonly AppDbContext _context;

    public ProcessoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Processo?> GetByIdAsync(Guid id)
    {
        return await _context.Processos
            .Include(p => p.Partes)
                .ThenInclude(p => p.EntidadeLegal)
            .Include(p => p.Andamentos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IEnumerable<Processo> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? numeroDocumentoParte = null, StatusProcesso? statusProcesso = null, string? numeroProcesso = null)
    {
        var query = _context.Processos
            .Include(p => p.Partes)
                .ThenInclude(p => p.EntidadeLegal)
            .Include(p => p.Andamentos)
            .AsNoTracking();

        if (statusProcesso.HasValue)
        {
            query = query.Where(p => p.Status == statusProcesso.Value);
        }

        if (!string.IsNullOrWhiteSpace(numeroDocumentoParte))
        {
            query = query.Where(p => p.Partes.Any(parte => !parte.FlagDeleted && parte.EntidadeLegal.NumeroDocumento == numeroDocumentoParte));
        }

        if (!string.IsNullOrWhiteSpace(numeroProcesso))
        {
            query = query.Where(p => p.NumeroProcesso.Contains(numeroProcesso));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.DataCriacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Processo processo)
    {
        await _context.Processos.AddAsync(processo);
    }

    public Task UpdateAsync(Processo processo)
    {
        if (_context.Entry(processo).State == EntityState.Detached)
        {
            _context.Processos.Update(processo);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Processo processo)
    {
        _context.Processos.Remove(processo);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<Parte> Items, int TotalCount)> GetPartesAsync(Guid processoId, int pageNumber, int pageSize)
    {
        var query = _context.Processos
            .Where(p => p.Id == processoId)
            .SelectMany(p => p.Partes)
            .Where(p => !p.FlagDeleted)
            .Include(p => p.EntidadeLegal)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Andamento> Items, int TotalCount)> GetAndamentosAsync(Guid processoId, int pageNumber, int pageSize)
    {
        var query = _context.Processos
            .Where(p => p.Id == processoId)
            .SelectMany(p => p.Andamentos)
            .Where(a => !a.FlagDeleted)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.Data)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
