using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.Property(t => t.ClientName).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Quote).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
    }
}