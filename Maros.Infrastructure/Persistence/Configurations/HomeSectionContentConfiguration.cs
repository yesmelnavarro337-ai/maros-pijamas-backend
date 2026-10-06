using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class HomeSectionContentConfiguration : IEntityTypeConfiguration<HomeSectionContent>
{
    public void Configure(EntityTypeBuilder<HomeSectionContent> builder)
    {
        builder.Property(s => s.SectionKey)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.SectionKey)
            .IsUnique();

        builder.Property(s => s.SectionTitle).HasMaxLength(200);
        builder.Property(s => s.SectionSubtitle).HasMaxLength(500);
        builder.Property(s => s.Eyebrow).HasMaxLength(80);
        builder.Property(s => s.BodyText).HasMaxLength(600);
        builder.Property(s => s.CtaText).HasMaxLength(80);
        builder.Property(s => s.CtaLink).HasMaxLength(300);
        builder.Property(s => s.MainImageUrl).HasMaxLength(500);
        builder.Property(s => s.MainImageAlt).HasMaxLength(180);
        builder.Property(s => s.CardImageUrl).HasMaxLength(500);
    }
}