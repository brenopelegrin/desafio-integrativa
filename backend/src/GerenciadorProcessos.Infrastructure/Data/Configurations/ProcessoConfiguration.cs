using GerenciadorProcessos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorProcessos.Infrastructure.Data.Configurations;

public class ProcessoConfiguration : IEntityTypeConfiguration<Processo>
{
    public void Configure(EntityTypeBuilder<Processo> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NumeroProcesso)
            .IsRequired()
            .HasMaxLength(25);

        builder.HasIndex(p => p.NumeroProcesso).IsUnique();

        builder.Property(p => p.Assunto)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        var navigationPartes = builder.Metadata.FindNavigation(nameof(Processo.Partes));
        navigationPartes?.SetPropertyAccessMode(PropertyAccessMode.Field);

        var navigationAndamentos = builder.Metadata.FindNavigation(nameof(Processo.Andamentos));
        navigationAndamentos?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(p => !p.FlagDeleted);
    }
}
