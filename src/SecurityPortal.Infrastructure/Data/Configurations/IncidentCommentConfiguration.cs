using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Infrastructure.Data.Configurations;

public class IncidentCommentConfiguration : IEntityTypeConfiguration<IncidentComment>
{
    public void Configure(EntityTypeBuilder<IncidentComment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.Author)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.IncidentId)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(100);

        // Index for efficient querying
        builder.HasIndex(c => c.IncidentId);
        builder.HasIndex(c => c.CreatedAt);

        builder.ToTable("IncidentComments");
    }
}