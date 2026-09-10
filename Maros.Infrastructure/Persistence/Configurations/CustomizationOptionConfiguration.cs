using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class CustomizationOptionConfiguration : IEntityTypeConfiguration<CustomizationOption>
{
    public void Configure(EntityTypeBuilder<CustomizationOption> builder)
    {
        builder.Property(o => o.CatalogType).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.Name).IsRequired().HasMaxLength(100);
        builder.Property(o => o.ColorHex).HasMaxLength(7);
        builder.Property(o => o.PriceModifier).HasColumnType("decimal(10,2)");
    }
}