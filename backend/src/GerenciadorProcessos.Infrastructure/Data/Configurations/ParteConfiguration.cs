using GerenciadorProcessos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorProcessos.Infrastructure.Data.Configurations;

public class ParteConfiguration : IEntityTypeConfiguration<Parte>
{
    public void Configure(EntityTypeBuilder<Parte> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Processo)
            .WithMany(p => p.Partes)
            .HasForeignKey(p => p.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.EntidadeLegal)
            .WithMany()
            .HasForeignKey(p => p.EntidadeLegalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.FlagDeleted);
    }
}
