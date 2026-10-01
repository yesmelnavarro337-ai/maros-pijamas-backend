using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.Property(i => i.Url).IsRequired();
        builder.Property(i => i.ColorHex).HasMaxLength(20);
        builder.Property(i => i.ColorName).HasMaxLength(100);

        // Filtro espejo al de Product (ver ProductCategoryConfiguration). Evita
        // además que las imágenes de un producto borrado se carguen al navegar.
        builder.HasQueryFilter(i => !i.Product.IsDeleted);
    }
}