using System.Threading.Tasks;
using GerenciadorProcessos.Domain.Interfaces;

namespace GerenciadorProcessos.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
