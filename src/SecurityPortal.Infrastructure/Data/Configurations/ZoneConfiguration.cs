using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Infrastructure.Data.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(z => z.Description)
            .HasMaxLength(1000);

        builder.Property(z => z.RiskLevel)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(z => z.IsActive)
            .IsRequired();

        builder.Property(z => z.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(z => z.UpdatedBy)
            .HasMaxLength(100);

        // Index for efficient querying
        builder.HasIndex(z => new { z.Name, z.RiskLevel });

        builder.ToTable("Zones");
    }
}