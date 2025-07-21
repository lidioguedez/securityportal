using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Infrastructure.Data.Configurations;

public class SecurityIncidentConfiguration : IEntityTypeConfiguration<SecurityIncident>
{
    public void Configure(EntityTypeBuilder<SecurityIncident> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(i => i.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.OccurredAt)
            .IsRequired();

        builder.Property(i => i.ReportedAt);

        builder.Property(i => i.ReportedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.AssignedTo)
            .HasMaxLength(100);

        builder.Property(i => i.ZoneId)
            .IsRequired();

        builder.Property(i => i.PlantId)
            .IsRequired();

        builder.Property(i => i.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.UpdatedBy)
            .HasMaxLength(100);

        // Value Object - Priority
        builder.OwnsOne(i => i.Priority, priorityBuilder =>
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

        // One-to-Many relationships
        builder.HasMany(i => i.Comments)
            .WithOne()
            .HasForeignKey("IncidentId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Attachments)
            .WithOne()
            .HasForeignKey("IncidentId")
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign Keys
        builder.HasOne<Plant>()
            .WithMany()
            .HasForeignKey(i => i.PlantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Zone>()
            .WithMany()
            .HasForeignKey(i => i.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.PlantId);
        builder.HasIndex(i => i.ZoneId);
        builder.HasIndex(i => i.OccurredAt);
        builder.HasIndex(i => i.AssignedTo);
        builder.HasIndex(i => new { i.Status, i.PlantId });

        // Ignore domain events for EF mapping
        builder.Ignore(i => i.DomainEvents);

        builder.ToTable("SecurityIncidents");
    }
}