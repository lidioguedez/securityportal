using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Infrastructure.Data.Configurations;

public class IncidentAttachmentConfiguration : IEntityTypeConfiguration<IncidentAttachment>
{
    public void Configure(EntityTypeBuilder<IncidentAttachment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.FilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.UploadedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.IncidentId)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(100);

        // Index for efficient querying
        builder.HasIndex(a => a.IncidentId);
        builder.HasIndex(a => a.CreatedAt);

        builder.ToTable("IncidentAttachments");
    }
}