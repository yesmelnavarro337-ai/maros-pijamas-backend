using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class ProductCollectionConfiguration : IEntityTypeConfiguration<ProductCollection>
{
    public void Configure(EntityTypeBuilder<ProductCollection> builder)
    {
        builder.HasKey(pc => new { pc.ProductId, pc.CollectionId });

        // Filtro espejo al de Product (ver ProductCategoryConfiguration).
        builder.HasQueryFilter(pc => !pc.Product.IsDeleted);

        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCollections)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Collection)
            .WithMany(c => c.ProductCollections)
            .HasForeignKey(pc => pc.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}