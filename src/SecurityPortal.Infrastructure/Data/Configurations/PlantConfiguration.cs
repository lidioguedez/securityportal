using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Infrastructure.Data.Configurations;

public class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.ActivatedAt);

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.UpdatedBy)
            .HasMaxLength(100);

        // Value Object - Address
        builder.OwnsOne(p => p.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200);

            addressBuilder.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100);

            addressBuilder.Property(a => a.State)
                .HasColumnName("Address_State")
                .HasMaxLength(50);

            addressBuilder.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(50);

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("Address_PostalCode")
                .HasMaxLength(20);
        });

        // One-to-Many relationship with Zones
        builder.HasMany(p => p.Zones)
            .WithOne()
            .HasForeignKey("PlantId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF mapping
        builder.Ignore(p => p.DomainEvents);

        builder.ToTable("Plants");
    }
}