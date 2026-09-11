using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class PageHeaderConfiguration : IEntityTypeConfiguration<PageHeader>
{
    public void Configure(EntityTypeBuilder<PageHeader> builder)
    {
        builder.ToTable("PageHeaders");

        builder.Property(h => h.PageKey).IsRequired().HasMaxLength(60);
        builder.HasIndex(h => h.PageKey).IsUnique();

        builder.Property(h => h.Title).IsRequired().HasMaxLength(200);
        builder.Property(h => h.Subtitle).HasMaxLength(500);
        builder.Property(h => h.BackgroundImageUrl).HasMaxLength(500);
        builder.Property(h => h.PrimaryButtonText).HasMaxLength(80);
        builder.Property(h => h.PrimaryButtonLink).HasMaxLength(300);
        builder.Property(h => h.SecondaryButtonText).HasMaxLength(80);
        builder.Property(h => h.SecondaryButtonLink).HasMaxLength(300);
        builder.Property(h => h.TextColor).HasMaxLength(20);
        builder.Property(h => h.OverlayOpacity).HasDefaultValue(40);
    }
}