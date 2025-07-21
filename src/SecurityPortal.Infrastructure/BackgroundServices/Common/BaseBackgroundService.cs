using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SecurityPortal.Infrastructure.BackgroundServices.Common;

/// <summary>
/// Base class for all background services providing common functionality like logging, error handling, and health monitoring.
/// </summary>
public abstract class BaseBackgroundService : BackgroundService
{
    protected readonly ILogger Logger;
    protected readonly IServiceProvider ServiceProvider;
    private readonly string _serviceName;

    protected BaseBackgroundService(ILogger logger, IServiceProvider serviceProvider)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _serviceName = GetType().Name;
    }

    /// <summary>
    /// Gets the current health status of the service
    /// </summary>
    public ServiceHealthStatus HealthStatus { get; private set; } = ServiceHealthStatus.Starting;

    /// <summary>
    /// Gets the last error that occurred in the service
    /// </summary>
    public Exception? LastError { get; private set; }

    /// <summary>
    /// Gets the timestamp when the service was last active
    /// </summary>
    public DateTime LastActivity { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Main execution method that wraps the service logic with error handling and health monitoring
    /// </summary>
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("{ServiceName} starting...", _serviceName);
        
        try
        {
            HealthStatus = ServiceHealthStatus.Running;
            await ExecuteServiceAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation("{ServiceName} cancellation requested", _serviceName);
            HealthStatus = ServiceHealthStatus.Stopped;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{ServiceName} encountered a fatal error", _serviceName);
            LastError = ex;
            HealthStatus = ServiceHealthStatus.Unhealthy;
            
            // Allow service to decide whether to retry or stop
            if (ShouldRetryOnError(ex))
            {
                Logger.LogInformation("{ServiceName} will retry after error", _serviceName);
                await Task.Delay(GetRetryDelay(), stoppingToken);
                HealthStatus = ServiceHealthStatus.Running;
                await ExecuteAsync(stoppingToken); // Retry
            }
            else
            {
                throw; // Rethrow to stop the service
            }
        }
        finally
        {
            Logger.LogInformation("{ServiceName} stopped", _serviceName);
            if (HealthStatus == ServiceHealthStatus.Running)
            {
                HealthStatus = ServiceHealthStatus.Stopped;
            }
        }
    }

    /// <summary>
    /// Override this method to implement the service logic
    /// </summary>
    protected abstract Task ExecuteServiceAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Determines whether the service should retry after an error
    /// </summary>
    protected virtual bool ShouldRetryOnError(Exception exception)
    {
        // By default, don't retry on critical errors
        return exception is not (ArgumentException or InvalidOperationException or NotSupportedException);
    }

    /// <summary>
    /// Gets the delay before retrying after an error
    /// </summary>
    protected virtual TimeSpan GetRetryDelay()
    {
        return TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// Updates the last activity timestamp - call this from derived classes
    /// </summary>
    protected void UpdateLastActivity()
    {
        LastActivity = DateTime.UtcNow;
    }

    /// <summary>
    /// Logs and handles an error without stopping the service
    /// </summary>
    protected void HandleError(Exception exception, string operation)
    {
        Logger.LogError(exception, "{ServiceName} error in {Operation}", _serviceName, operation);
        LastError = exception;
        
        // Don't change health status to unhealthy for handled errors
        // This allows the service to continue running
    }

    /// <summary>
    /// Creates a scoped service provider for database operations
    /// </summary>
    protected IServiceScope CreateScope()
    {
        return ServiceProvider.CreateScope();
    }
}

/// <summary>
/// Represents the health status of a background service
/// </summary>
public enum ServiceHealthStatus
{
    Starting,
    Running,
    Unhealthy,
    Stopped
}