using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace SecurityPortal.Infrastructure.BackgroundServices.Common;

/// <summary>
/// Manages the lifecycle and health monitoring of all background services
/// </summary>
public class BackgroundServiceManager : IHostedService
{
    private readonly ILogger<BackgroundServiceManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, ServiceInfo> _services = new();

    public BackgroundServiceManager(ILogger<BackgroundServiceManager> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets information about all registered background services
    /// </summary>
    public IReadOnlyDictionary<string, ServiceInfo> Services => _services;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background Service Manager starting...");
        
        // Discover all BaseBackgroundService instances
        var hostedServices = _serviceProvider.GetServices<IHostedService>()
            .OfType<BaseBackgroundService>()
            .ToList();

        foreach (var service in hostedServices)
        {
            var serviceName = service.GetType().Name;
            var serviceInfo = new ServiceInfo
            {
                Name = serviceName,
                Type = service.GetType(),
                Instance = service,
                StartTime = DateTime.UtcNow
            };
            
            _services.TryAdd(serviceName, serviceInfo);
            _logger.LogInformation("Registered background service: {ServiceName}", serviceName);
        }

        _logger.LogInformation("Background Service Manager started with {ServiceCount} services", _services.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background Service Manager stopping...");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the health status of a specific service
    /// </summary>
    public ServiceHealthStatus? GetServiceHealth(string serviceName)
    {
        return _services.TryGetValue(serviceName, out var serviceInfo) 
            ? serviceInfo.Instance.HealthStatus 
            : null;
    }

    /// <summary>
    /// Gets the overall health status of all services
    /// </summary>
    public OverallHealthStatus GetOverallHealth()
    {
        if (!_services.Any())
            return new OverallHealthStatus { Status = "Healthy", Message = "No services registered" };

        var unhealthyServices = _services.Values
            .Where(s => s.Instance.HealthStatus == ServiceHealthStatus.Unhealthy)
            .ToList();

        var stoppedServices = _services.Values
            .Where(s => s.Instance.HealthStatus == ServiceHealthStatus.Stopped)
            .ToList();

        if (unhealthyServices.Any())
        {
            return new OverallHealthStatus
            {
                Status = "Unhealthy",
                Message = $"{unhealthyServices.Count} service(s) are unhealthy",
                UnhealthyServices = unhealthyServices.Select(s => s.Name).ToList()
            };
        }

        if (stoppedServices.Any())
        {
            return new OverallHealthStatus
            {
                Status = "Degraded",
                Message = $"{stoppedServices.Count} service(s) are stopped",
                StoppedServices = stoppedServices.Select(s => s.Name).ToList()
            };
        }

        return new OverallHealthStatus { Status = "Healthy", Message = "All services running normally" };
    }

    /// <summary>
    /// Gets detailed information about all services
    /// </summary>
    public List<ServiceDetailInfo> GetServiceDetails()
    {
        return _services.Values.Select(s => new ServiceDetailInfo
        {
            Name = s.Name,
            Type = s.Type.FullName ?? s.Type.Name,
            Status = s.Instance.HealthStatus.ToString(),
            StartTime = s.StartTime,
            LastActivity = s.Instance.LastActivity,
            LastError = s.Instance.LastError?.Message,
            Uptime = DateTime.UtcNow - s.StartTime
        }).ToList();
    }
}

/// <summary>
/// Information about a registered background service
/// </summary>
public class ServiceInfo
{
    public required string Name { get; init; }
    public required Type Type { get; init; }
    public required BaseBackgroundService Instance { get; init; }
    public required DateTime StartTime { get; init; }
}

/// <summary>
/// Overall health status of all background services
/// </summary>
public class OverallHealthStatus
{
    public required string Status { get; init; }
    public required string Message { get; init; }
    public List<string> UnhealthyServices { get; init; } = new();
    public List<string> StoppedServices { get; init; } = new();
}

/// <summary>
/// Detailed information about a background service
/// </summary>
public class ServiceDetailInfo
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime LastActivity { get; init; }
    public string? LastError { get; init; }
    public required TimeSpan Uptime { get; init; }
}