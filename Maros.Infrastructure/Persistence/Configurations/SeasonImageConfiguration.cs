using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class SeasonImageConfiguration : IEntityTypeConfiguration<SeasonImage>
{
    public void Configure(EntityTypeBuilder<SeasonImage> builder)
    {
        builder.Property(i => i.ImageUrl).IsRequired();

        // Orden de lectura del carrusel del Home: por SeasonId y Order.
        // No es único a propósito: durante un sync EF puede aplicar los Updates
        // de Order en un orden que violaría la restricción de forma transitoria.
        builder.HasIndex(i => new { i.SeasonId, i.Order });

        // Invariante "una sola portada por temporada", garantizada en la base de
        // datos. El índice es parcial: solo las filas con IsPrimary = true entran,
        // así que las imágenes adicionales pueden compartir SeasonId sin conflicto.
        builder.HasIndex(i => i.SeasonId)
            .IsUnique()
            .HasFilter("\"IsPrimary\" = true");
    }
}