using Maros.Domain.Entities;
using Maros.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class StyleConfiguration : IEntityTypeConfiguration<Style>
{
    public void Configure(EntityTypeBuilder<Style> builder)
    {
        builder.ToTable("Styles");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Slug)
            .IsUnique();

        builder.Property(s => s.Slug)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(s => s.HexCode)
            .HasMaxLength(20);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        // Los estilos preexistentes son de la línea de adultos: el valor por
        // defecto 0 evita tener que hacer backfill en la migración.
        builder.Property(s => s.Line)
            .IsRequired()
            .HasDefaultValue(StyleLine.Adulto)
            .HasConversion<int>();
    }
}