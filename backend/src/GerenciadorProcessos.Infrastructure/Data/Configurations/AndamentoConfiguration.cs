using GerenciadorProcessos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorProcessos.Infrastructure.Data.Configurations;

public class AndamentoConfiguration : IEntityTypeConfiguration<Andamento>
{
    public void Configure(EntityTypeBuilder<Andamento> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Descricao)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.Data)
            .IsRequired();

        builder.HasOne(a => a.Processo)
            .WithMany(p => p.Andamentos)
            .HasForeignKey(a => a.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(a => !a.FlagDeleted);
    }
}
