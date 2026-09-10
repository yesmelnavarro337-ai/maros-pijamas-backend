using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class QuotationItemOptionConfiguration : IEntityTypeConfiguration<QuotationItemOption>
{
    public void Configure(EntityTypeBuilder<QuotationItemOption> builder)
    {
        builder.HasKey(o => new { o.QuotationItemId, o.CustomizationOptionId });

        builder.HasOne(o => o.QuotationItem)
            .WithMany(i => i.SelectedOptions)
            .HasForeignKey(o => o.QuotationItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.CustomizationOption)
            .WithMany()
            .HasForeignKey(o => o.CustomizationOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}