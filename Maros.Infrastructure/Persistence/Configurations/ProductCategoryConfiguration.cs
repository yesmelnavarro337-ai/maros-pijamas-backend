using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(pc => new { pc.ProductId, pc.CategoryId });

        builder.HasIndex(pc => pc.CategoryId);

        builder.Property(pc => pc.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(pc => pc.SurchargeReason)
            .HasMaxLength(500);

        // Filtro espejo al de Product: si el producto se borró lógicamente, su fila de
        // unión no debe aparecer. Sin esto, Product es el extremo requerido de la
        // relación y EF Core advierte 10622 al modelar.
        builder.HasQueryFilter(pc => !pc.Product.IsDeleted);

        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
