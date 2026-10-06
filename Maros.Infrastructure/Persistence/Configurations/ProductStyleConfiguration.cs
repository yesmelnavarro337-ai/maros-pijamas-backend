using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class ProductStyleConfiguration : IEntityTypeConfiguration<ProductStyle>
{
    public void Configure(EntityTypeBuilder<ProductStyle> builder)
    {
        builder.ToTable("ProductStyles");

        builder.HasKey(ps => new { ps.ProductId, ps.StyleId });

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.ProductStyles)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Style)
            .WithMany(s => s.ProductStyles)
            .HasForeignKey(ps => ps.StyleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}