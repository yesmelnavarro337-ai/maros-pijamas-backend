using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.Property(v => v.Size).IsRequired().HasMaxLength(20);
        builder.Property(v => v.ColorName).IsRequired().HasMaxLength(60);
        builder.Property(v => v.ColorHex).IsRequired().HasMaxLength(7);
        builder.Property(v => v.Sku).IsRequired().HasMaxLength(60);

        builder.HasIndex(v => v.Sku).IsUnique();
    }
}