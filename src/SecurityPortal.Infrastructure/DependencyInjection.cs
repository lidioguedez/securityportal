using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Infrastructure.Data;
using SecurityPortal.Infrastructure.Repositories;
using SecurityPortal.Infrastructure.BackgroundServices.Common;
using SecurityPortal.Infrastructure.BackgroundServices.DomainEventProcessors;
using SecurityPortal.Infrastructure.BackgroundServices.ScheduledServices;
using SecurityPortal.Infrastructure.BackgroundServices.MonitoringServices;

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

        // Background Services Infrastructure
        services.AddBackgroundServices(configuration);

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

    /// <summary>
    /// Registers all background services and related infrastructure
    /// </summary>
    public static IServiceCollection AddBackgroundServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Background Service Manager for health monitoring and lifecycle management
        services.AddSingleton<BackgroundServiceManager>();
        services.AddHostedService<BackgroundServiceManager>();

        // Domain Event Processors
        services.AddHostedService<IncidentEventProcessor>();
        services.AddHostedService<AlertEventProcessor>();
        services.AddSingleton<NotificationService>();
        services.AddHostedService<NotificationService>();

        // Scheduled Services
        services.AddHostedService<ReportGenerationService>();

        // Monitoring Services
        services.AddSingleton<DashboardMetricsService>();
        services.AddHostedService<DashboardMetricsService>();

        // Background Service Health Checks
        services.AddBackgroundServiceHealthChecks();

        // Configuration Options
        services.Configure<NotificationOptions>(
            configuration.GetSection("BackgroundServices:Notifications"));
        services.Configure<ReportGenerationOptions>(
            configuration.GetSection("BackgroundServices:ReportGeneration"));

        // Domain Event Handlers for MediatR
        services.AddMediatREventHandlers();

        return services;
    }

    /// <summary>
    /// Registers MediatR domain event handlers for background processing
    /// </summary>
    private static IServiceCollection AddMediatREventHandlers(this IServiceCollection services)
    {
        // Incident Event Handlers
        services.AddScoped<IncidentCreatedEventHandler>();
        services.AddScoped<IncidentStatusChangedEventHandler>();
        
        // Alert Event Handlers
        services.AddScoped<AlertTriggeredEventHandler>();
        services.AddScoped<AlertAcknowledgedEventHandler>();
        services.AddScoped<AlertEscalatedEventHandler>();

        return services;
    }
}