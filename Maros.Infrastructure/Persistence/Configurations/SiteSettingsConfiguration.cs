using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
{
    public void Configure(EntityTypeBuilder<SiteSettings> builder)
    {
        builder.Property(s => s.SiteName).IsRequired().HasMaxLength(150);
        builder.Property(s => s.WhatsappNumber).HasMaxLength(30);
        builder.Property(s => s.Address).HasMaxLength(300);
        builder.Property(s => s.BusinessHours).HasMaxLength(200);
        builder.Property(s => s.EmailFromAddress).HasMaxLength(256);
        builder.Property(s => s.CustomDomain).HasMaxLength(150);
    }
}