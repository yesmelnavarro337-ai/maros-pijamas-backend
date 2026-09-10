using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class QuotationReferenceImageConfiguration : IEntityTypeConfiguration<QuotationReferenceImage>
{
    public void Configure(EntityTypeBuilder<QuotationReferenceImage> builder)
    {
        builder.Property(r => r.Url).IsRequired();
    }
}