using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace SecurityPortal.Infrastructure.BackgroundServices.Common;

/// <summary>
/// Health check extensions for background services
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds health checks for background services
    /// </summary>
    public static IServiceCollection AddBackgroundServiceHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<BackgroundServicesHealthCheck>("background-services");

        return services;
    }
}

/// <summary>
/// Health check for all background services
/// </summary>
public class BackgroundServicesHealthCheck : IHealthCheck
{
    private readonly BackgroundServiceManager _serviceManager;
    private readonly ILogger<BackgroundServicesHealthCheck> _logger;

    public BackgroundServicesHealthCheck(
        BackgroundServiceManager serviceManager,
        ILogger<BackgroundServicesHealthCheck> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var overallHealth = _serviceManager.GetOverallHealth();
            var serviceDetails = _serviceManager.GetServiceDetails();

            var data = new Dictionary<string, object>
            {
                ["status"] = overallHealth.Status,
                ["message"] = overallHealth.Message,
                ["totalServices"] = serviceDetails.Count,
                ["runningServices"] = serviceDetails.Count(s => s.Status == "Running"),
                ["unhealthyServices"] = overallHealth.UnhealthyServices,
                ["stoppedServices"] = overallHealth.StoppedServices
            };

            // Add individual service details
            foreach (var service in serviceDetails)
            {
                data[$"service_{service.Name}_status"] = service.Status;
                data[$"service_{service.Name}_uptime"] = service.Uptime.ToString(@"dd\.hh\:mm\:ss");
                data[$"service_{service.Name}_lastActivity"] = service.LastActivity;
                
                if (!string.IsNullOrEmpty(service.LastError))
                {
                    data[$"service_{service.Name}_lastError"] = service.LastError;
                }
            }

            var healthStatus = overallHealth.Status switch
            {
                "Healthy" => HealthStatus.Healthy,
                "Degraded" => HealthStatus.Degraded,
                "Unhealthy" => HealthStatus.Unhealthy,
                _ => HealthStatus.Unhealthy
            };

            return Task.FromResult(new HealthCheckResult(
                healthStatus,
                overallHealth.Message,
                data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking background services health");
            
            return Task.FromResult(new HealthCheckResult(
                HealthStatus.Unhealthy,
                "Error checking background services health",
                ex,
                new Dictionary<string, object> { ["error"] = ex.Message }));
        }
    }
}

/// <summary>
/// Health check for a specific background service
/// </summary>
public class SingleBackgroundServiceHealthCheck : IHealthCheck
{
    private readonly BackgroundServiceManager _serviceManager;
    private readonly string _serviceName;
    private readonly ILogger<SingleBackgroundServiceHealthCheck> _logger;

    public SingleBackgroundServiceHealthCheck(
        BackgroundServiceManager serviceManager,
        string serviceName,
        ILogger<SingleBackgroundServiceHealthCheck> logger)
    {
        _serviceManager = serviceManager;
        _serviceName = serviceName;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var healthStatus = _serviceManager.GetServiceHealth(_serviceName);
            
            if (healthStatus == null)
            {
                return Task.FromResult(new HealthCheckResult(
                    HealthStatus.Unhealthy,
                    $"Service '{_serviceName}' not found",
                    data: new Dictionary<string, object> { ["serviceName"] = _serviceName }));
            }

            var serviceDetails = _serviceManager.GetServiceDetails()
                .FirstOrDefault(s => s.Name == _serviceName);

            var data = new Dictionary<string, object>
            {
                ["serviceName"] = _serviceName,
                ["status"] = healthStatus.ToString()
            };

            if (serviceDetails != null)
            {
                data["uptime"] = serviceDetails.Uptime.ToString(@"dd\.hh\:mm\:ss");
                data["lastActivity"] = serviceDetails.LastActivity;
                data["startTime"] = serviceDetails.StartTime;
                
                if (!string.IsNullOrEmpty(serviceDetails.LastError))
                {
                    data["lastError"] = serviceDetails.LastError;
                }
            }

            var status = healthStatus switch
            {
                ServiceHealthStatus.Running => HealthStatus.Healthy,
                ServiceHealthStatus.Starting => HealthStatus.Degraded,
                ServiceHealthStatus.Stopped => HealthStatus.Degraded,
                ServiceHealthStatus.Unhealthy => HealthStatus.Unhealthy,
                _ => HealthStatus.Unhealthy
            };

            var message = healthStatus switch
            {
                ServiceHealthStatus.Running => $"Service '{_serviceName}' is running normally",
                ServiceHealthStatus.Starting => $"Service '{_serviceName}' is starting",
                ServiceHealthStatus.Stopped => $"Service '{_serviceName}' is stopped",
                ServiceHealthStatus.Unhealthy => $"Service '{_serviceName}' is unhealthy",
                _ => $"Service '{_serviceName}' status unknown"
            };

            return Task.FromResult(new HealthCheckResult(status, message, data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health for service {ServiceName}", _serviceName);
            
            return Task.FromResult(new HealthCheckResult(
                HealthStatus.Unhealthy,
                $"Error checking health for service '{_serviceName}'",
                ex,
                new Dictionary<string, object> 
                { 
                    ["serviceName"] = _serviceName,
                    ["error"] = ex.Message 
                }));
        }
    }
}