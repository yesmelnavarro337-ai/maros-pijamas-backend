using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.LinkUrl).HasMaxLength(300);
        builder.Property(b => b.Position).HasMaxLength(60);

        builder.HasOne(b => b.Season)
            .WithMany()
            .HasForeignKey(b => b.SeasonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Collection)
            .WithMany()
            .HasForeignKey(b => b.CollectionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}