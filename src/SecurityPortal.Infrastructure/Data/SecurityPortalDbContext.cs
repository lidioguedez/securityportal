using Microsoft.EntityFrameworkCore;
using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Infrastructure.Data.Configurations;
using System.Reflection;

namespace SecurityPortal.Infrastructure.Data;

public class SecurityPortalDbContext : DbContext
{
    public SecurityPortalDbContext(DbContextOptions<SecurityPortalDbContext> options) : base(options)
    {
    }

    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<SecurityIncident> SecurityIncidents => Set<SecurityIncident>();
    public DbSet<IncidentComment> IncidentComments => Set<IncidentComment>();
    public DbSet<IncidentAttachment> IncidentAttachments => Set<IncidentAttachment>();
    public DbSet<SecurityAlert> SecurityAlerts => Set<SecurityAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filters for soft delete if needed
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configure common base entity properties
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }

        // Dispatch domain events
        var domainEvents = entries
            .Where(x => x.Entity is BaseAggregateRoot)
            .SelectMany(x => ((BaseAggregateRoot)x.Entity).DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Clear domain events after save
        foreach (var entry in entries.Where(x => x.Entity is BaseAggregateRoot))
        {
            ((BaseAggregateRoot)entry.Entity).ClearDomainEvents();
        }

        return result;
    }
}