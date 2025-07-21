using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecurityPortal.Infrastructure.Data;

/// <summary>
/// Design-time factory for SecurityPortalDbContext.
/// This is used by EF Core tools (migrations, scaffolding) when they need to create a DbContext instance.
/// </summary>
public class SecurityPortalDbContextFactory : IDesignTimeDbContextFactory<SecurityPortalDbContext>
{
    public SecurityPortalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SecurityPortalDbContext>();
        
        // Use SQLite for design-time operations (migrations work with any provider)
        // In production, this will be overridden by the actual connection string from configuration
        optionsBuilder.UseSqlite("Data Source=design_time.db");
        
        return new SecurityPortalDbContext(optionsBuilder.Options);
    }
}