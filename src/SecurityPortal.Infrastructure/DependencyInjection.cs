using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Infrastructure.Data;
using SecurityPortal.Infrastructure.Repositories;

namespace SecurityPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<SecurityPortalDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SecurityPortalDbContext).Assembly.FullName)));

        // Repository registrations
        services.AddScoped<ISecurityIncidentRepository, SecurityIncidentRepository>();
        services.AddScoped<ISecurityAlertRepository, SecurityAlertRepository>();
        services.AddScoped<IPlantRepository, PlantRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Background Services (if needed)
        // services.AddHostedService<SampleBackgroundService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServicesWithInMemoryDatabase(
        this IServiceCollection services)
    {
        // In-Memory Database for testing
        services.AddDbContext<SecurityPortalDbContext>(options =>
            options.UseInMemoryDatabase("SecurityPortalTestDb"));

        // Repository registrations
        services.AddScoped<ISecurityIncidentRepository, SecurityIncidentRepository>();
        services.AddScoped<ISecurityAlertRepository, SecurityAlertRepository>();
        services.AddScoped<IPlantRepository, PlantRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}