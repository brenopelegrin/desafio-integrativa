using System.Threading.Tasks;

namespace GerenciadorProcessos.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<bool> CommitAsync();
}
