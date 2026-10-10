using Maros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maros.Infrastructure.Persistence.Configurations;

public class CustomizationOptionAssignmentConfiguration : IEntityTypeConfiguration<CustomizationOptionAssignment>
{
    public void Configure(EntityTypeBuilder<CustomizationOptionAssignment> builder)
    {
        builder.ToTable("CustomizationOptionAssignments");
        builder.HasKey(a => new { a.ModelId, a.OptionId });

        builder.HasOne(a => a.Option)
            .WithMany()
            .HasForeignKey(a => a.OptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
