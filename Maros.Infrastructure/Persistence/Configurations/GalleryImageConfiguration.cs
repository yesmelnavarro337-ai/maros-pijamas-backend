using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class GalleryImageConfiguration : IEntityTypeConfiguration<GalleryImage>
{
    public void Configure(EntityTypeBuilder<GalleryImage> builder)
    {
        builder.Property(g => g.Url).IsRequired();
        builder.Property(g => g.PublicId).IsRequired().HasMaxLength(300);
        builder.Property(g => g.Category).HasConversion<string>().HasMaxLength(20);
        builder.Property(g => g.Caption).HasMaxLength(300);
    }
}