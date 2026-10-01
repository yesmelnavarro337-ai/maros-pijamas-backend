using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class SeasonFeaturedProductConfiguration : IEntityTypeConfiguration<SeasonFeaturedProduct>
{
    public void Configure(EntityTypeBuilder<SeasonFeaturedProduct> builder)
    {
        builder.HasKey(sp => new { sp.SeasonId, sp.ProductId });

        // Filtro espejo al de Product (ver ProductCategoryConfiguration).
        builder.HasQueryFilter(sp => !sp.Product.IsDeleted);

        builder.HasOne(sp => sp.Season)
            .WithMany(s => s.FeaturedProducts)
            .HasForeignKey(sp => sp.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Product)
            .WithMany()
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}