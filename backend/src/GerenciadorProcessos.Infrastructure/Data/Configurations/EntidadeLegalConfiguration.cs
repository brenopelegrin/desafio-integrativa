using GerenciadorProcessos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorProcessos.Infrastructure.Data.Configurations;

public class EntidadeLegalConfiguration : IEntityTypeConfiguration<EntidadeLegal>
{
    public void Configure(EntityTypeBuilder<EntidadeLegal> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(14); // 11 para o CPF, 14 para o CNPJ

        builder.HasIndex(e => e.NumeroDocumento).IsUnique();
    }
}
