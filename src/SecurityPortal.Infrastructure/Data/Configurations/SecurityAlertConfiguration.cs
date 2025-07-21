using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Infrastructure.Data.Configurations;

public class SecurityAlertConfiguration : IEntityTypeConfiguration<SecurityAlert>
{
    public void Configure(EntityTypeBuilder<SecurityAlert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();


        builder.Property(a => a.TriggeredAt)
            .IsRequired();

        builder.Property(a => a.AcknowledgedAt);

        builder.Property(a => a.AcknowledgedBy)
            .HasMaxLength(100);

        builder.Property(a => a.ZoneId);

        builder.Property(a => a.PlantId)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(100);

        // Value Object - Priority
        builder.OwnsOne(a => a.Priority, priorityBuilder =>
        {
            priorityBuilder.Property(p => p.Level)
                .HasColumnName("Priority_Level")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            priorityBuilder.Property(p => p.Description)
                .HasColumnName("Priority_Description")
                .HasMaxLength(100)
                .IsRequired();
        });

        // Foreign Keys
        builder.HasOne<Plant>()
            .WithMany()
            .HasForeignKey(a => a.PlantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Zone>()
            .WithMany()
            .HasForeignKey(a => a.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.PlantId);
        builder.HasIndex(a => a.ZoneId);
        builder.HasIndex(a => a.TriggeredAt);
        builder.HasIndex(a => new { a.Status, a.PlantId });

        // Ignore domain events for EF mapping
        builder.Ignore(a => a.DomainEvents);

        builder.ToTable("SecurityAlerts");
    }
}