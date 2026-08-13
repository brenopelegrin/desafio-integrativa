using GerenciadorProcessos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorProcessos.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Processo> Processos { get; set; }
    public DbSet<Parte> Partes { get; set; }
    public DbSet<Andamento> Andamentos { get; set; }
    public DbSet<EntidadeLegal> EntidadesLegais { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
