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
        builder.Property(o => o.Description).HasMaxLength(250);
        builder.Property(o => o.Category).HasMaxLength(100);
        builder.Property(o => o.ColorHex).HasMaxLength(7);
        builder.Property(o => o.PriceModifier).HasColumnType("decimal(10,2)");

        builder.HasMany(o => o.AssignedOptions)
            .WithOne(a => a.Model)
            .HasForeignKey(a => a.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
