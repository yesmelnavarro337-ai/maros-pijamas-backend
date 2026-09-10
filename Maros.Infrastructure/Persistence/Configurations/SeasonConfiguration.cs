using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Slug).IsRequired().HasMaxLength(170);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

        builder.Property(s => s.HeroTitle).HasMaxLength(200);
        builder.Property(s => s.HeroSubtitle).HasMaxLength(300);

        builder.Property(s => s.ColorPrimary).IsRequired().HasMaxLength(7);
        builder.Property(s => s.ColorAccent).IsRequired().HasMaxLength(7);
        builder.Property(s => s.ColorBackground).IsRequired().HasMaxLength(7);

        builder.Property(s => s.CtaText).HasMaxLength(60);
        builder.Property(s => s.CtaLink).HasMaxLength(200);

        builder.HasIndex(s => s.Slug).IsUnique();

        builder.HasOne(s => s.Collection)
            .WithMany(c => c.Seasons)
            .HasForeignKey(s => s.CollectionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}