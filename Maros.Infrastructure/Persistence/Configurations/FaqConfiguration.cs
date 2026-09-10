using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.Property(f => f.Question).IsRequired().HasMaxLength(300);
        builder.Property(f => f.Answer).IsRequired().HasMaxLength(1000);
        builder.Property(f => f.Category).HasMaxLength(60);
        builder.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);
    }
}