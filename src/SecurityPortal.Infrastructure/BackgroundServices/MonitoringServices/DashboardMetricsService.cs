using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SecurityPortal.Infrastructure.BackgroundServices.Common;
using System.Collections.Concurrent;

namespace SecurityPortal.Infrastructure.BackgroundServices.MonitoringServices;

/// <summary>
/// Real-time dashboard metrics service that continuously updates KPIs and sends updates via SignalR
/// </summary>
public class DashboardMetricsService : BaseBackgroundService
{
    private readonly ConcurrentDictionary<string, MetricValue> _currentMetrics = new();
    private readonly ScheduleOptions _metricsUpdateSchedule;

    public DashboardMetricsService(
        ILogger<DashboardMetricsService> logger,
        IServiceProvider serviceProvider) 
        : base(logger, serviceProvider)
    {
        // Update metrics every 30 seconds
        _metricsUpdateSchedule = ScheduleOptions.Every(TimeSpan.FromSeconds(30));
    }

    protected override async Task ExecuteServiceAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Dashboard Metrics Service started");

        await foreach (var updateTime in _metricsUpdateSchedule.CreateScheduledTimer(stoppingToken))
        {
            try
            {
                UpdateLastActivity();
                Logger.LogDebug("Updating dashboard metrics at {UpdateTime}", updateTime);
                
                using var scope = CreateScope();
                await UpdateAllMetrics(scope, stoppingToken);
                await BroadcastMetricsUpdate(scope, stoppingToken);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                HandleError(ex, "UpdatingDashboardMetrics");
                // Continue running even if one update fails
            }
        }
    }

    /// <summary>
    /// Gets current metric values
    /// </summary>
    public IReadOnlyDictionary<string, MetricValue> GetCurrentMetrics()
    {
        return _currentMetrics;
    }

    /// <summary>
    /// Gets a specific metric value
    /// </summary>
    public MetricValue? GetMetric(string metricName)
    {
        return _currentMetrics.TryGetValue(metricName, out var metric) ? metric : null;
    }

    private async Task UpdateAllMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        // Update all KPI metrics in parallel for better performance
        var metricTasks = new List<Task>
        {
            UpdateSecurityIncidentMetrics(scope, cancellationToken),
            UpdateSecurityAlertMetrics(scope, cancellationToken),
            UpdateSystemHealthMetrics(scope, cancellationToken),
            UpdateComplianceMetrics(scope, cancellationToken),
            UpdatePerformanceMetrics(scope, cancellationToken),
            UpdateRiskMetrics(scope, cancellationToken)
        };

        await Task.WhenAll(metricTasks);
        
        Logger.LogDebug("Updated {MetricCount} metric categories", metricTasks.Count);
    }

    #region Security Incident Metrics

    private async Task UpdateSecurityIncidentMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            // These would typically query the database
            var totalIncidents = await GetTotalIncidents(scope, cancellationToken);
            var openIncidents = await GetOpenIncidents(scope, cancellationToken);
            var criticalIncidents = await GetCriticalIncidents(scope, cancellationToken);
            var avgResolutionTime = await GetAverageResolutionTime(scope, cancellationToken);
            var incidentsLast24h = await GetIncidentsLast24Hours(scope, cancellationToken);

            UpdateMetric("incidents.total", totalIncidents, "Total Incidents");
            UpdateMetric("incidents.open", openIncidents, "Open Incidents");
            UpdateMetric("incidents.critical", criticalIncidents, "Critical Incidents");
            UpdateMetric("incidents.avg_resolution_hours", avgResolutionTime, "Avg Resolution Time (hours)");
            UpdateMetric("incidents.last_24h", incidentsLast24h, "Incidents (Last 24h)");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating security incident metrics");
        }
    }

    private async Task<double> GetTotalIncidents(IServiceScope scope, CancellationToken cancellationToken)
    {
        // Simulate database query
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(100, 500); // Placeholder
    }

    private async Task<double> GetOpenIncidents(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(5, 25); // Placeholder
    }

    private async Task<double> GetCriticalIncidents(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(0, 5); // Placeholder
    }

    private async Task<double> GetAverageResolutionTime(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(2, 48); // Placeholder
    }

    private async Task<double> GetIncidentsLast24Hours(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(0, 10); // Placeholder
    }

    #endregion

    #region Security Alert Metrics

    private async Task UpdateSecurityAlertMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var totalAlerts = await GetTotalAlerts(scope, cancellationToken);
            var activeAlerts = await GetActiveAlerts(scope, cancellationToken);
            var criticalAlerts = await GetCriticalAlerts(scope, cancellationToken);
            var alertsLast24h = await GetAlertsLast24Hours(scope, cancellationToken);
            var falsePositiveRate = await GetFalsePositiveRate(scope, cancellationToken);

            UpdateMetric("alerts.total", totalAlerts, "Total Alerts");
            UpdateMetric("alerts.active", activeAlerts, "Active Alerts");
            UpdateMetric("alerts.critical", criticalAlerts, "Critical Alerts");
            UpdateMetric("alerts.last_24h", alertsLast24h, "Alerts (Last 24h)");
            UpdateMetric("alerts.false_positive_rate", falsePositiveRate, "False Positive Rate (%)");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating security alert metrics");
        }
    }

    private async Task<double> GetTotalAlerts(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(500, 2000); // Placeholder
    }

    private async Task<double> GetActiveAlerts(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(10, 50); // Placeholder
    }

    private async Task<double> GetCriticalAlerts(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(0, 10); // Placeholder
    }

    private async Task<double> GetAlertsLast24Hours(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(5, 30); // Placeholder
    }

    private async Task<double> GetFalsePositiveRate(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.NextDouble() * 15; // Placeholder: 0-15%
    }

    #endregion

    #region System Health Metrics

    private async Task UpdateSystemHealthMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var systemUptime = await GetSystemUptime(scope, cancellationToken);
            var cpuUsage = await GetCpuUsage(scope, cancellationToken);
            var memoryUsage = await GetMemoryUsage(scope, cancellationToken);
            var databaseConnections = await GetDatabaseConnections(scope, cancellationToken);
            var apiResponseTime = await GetApiResponseTime(scope, cancellationToken);

            UpdateMetric("system.uptime_hours", systemUptime, "System Uptime (hours)");
            UpdateMetric("system.cpu_usage", cpuUsage, "CPU Usage (%)");
            UpdateMetric("system.memory_usage", memoryUsage, "Memory Usage (%)");
            UpdateMetric("system.db_connections", databaseConnections, "Database Connections");
            UpdateMetric("system.api_response_time_ms", apiResponseTime, "API Response Time (ms)");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating system health metrics");
        }
    }

    private async Task<double> GetSystemUptime(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Environment.TickCount64 / (1000.0 * 60 * 60); // Uptime in hours
    }

    private async Task<double> GetCpuUsage(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.NextDouble() * 100; // Placeholder
    }

    private async Task<double> GetMemoryUsage(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.NextDouble() * 100; // Placeholder
    }

    private async Task<double> GetDatabaseConnections(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.Next(5, 50); // Placeholder
    }

    private async Task<double> GetApiResponseTime(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.Next(50, 500); // Placeholder
    }

    #endregion

    #region Compliance & Risk Metrics

    private async Task UpdateComplianceMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var complianceScore = await GetComplianceScore(scope, cancellationToken);
            var openFindings = await GetOpenFindings(scope, cancellationToken);
            var criticalFindings = await GetCriticalFindings(scope, cancellationToken);

            UpdateMetric("compliance.score", complianceScore, "Compliance Score (%)");
            UpdateMetric("compliance.open_findings", openFindings, "Open Findings");
            UpdateMetric("compliance.critical_findings", criticalFindings, "Critical Findings");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating compliance metrics");
        }
    }

    private async Task UpdateRiskMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var overallRiskScore = await GetOverallRiskScore(scope, cancellationToken);
            var highRiskZones = await GetHighRiskZones(scope, cancellationToken);
            var riskTrend = await GetRiskTrend(scope, cancellationToken);

            UpdateMetric("risk.overall_score", overallRiskScore, "Overall Risk Score");
            UpdateMetric("risk.high_risk_zones", highRiskZones, "High Risk Zones");
            UpdateMetric("risk.trend", riskTrend, "Risk Trend (%)");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating risk metrics");
        }
    }

    private async Task<double> GetComplianceScore(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.NextDouble() * 40 + 60; // 60-100%
    }

    private async Task<double> GetOpenFindings(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(0, 20);
    }

    private async Task<double> GetCriticalFindings(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(0, 5);
    }

    private async Task<double> GetOverallRiskScore(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.NextDouble() * 10; // 0-10 scale
    }

    private async Task<double> GetHighRiskZones(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Random.Shared.Next(0, 10);
    }

    private async Task<double> GetRiskTrend(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return (Random.Shared.NextDouble() - 0.5) * 20; // -10% to +10%
    }

    #endregion

    #region Performance Metrics

    private async Task UpdatePerformanceMetrics(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var throughput = await GetThroughput(scope, cancellationToken);
            var errorRate = await GetErrorRate(scope, cancellationToken);
            var availabilityScore = await GetAvailabilityScore(scope, cancellationToken);

            UpdateMetric("performance.throughput", throughput, "Throughput (req/sec)");
            UpdateMetric("performance.error_rate", errorRate, "Error Rate (%)");
            UpdateMetric("performance.availability", availabilityScore, "Availability (%)");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating performance metrics");
        }
    }

    private async Task<double> GetThroughput(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.NextDouble() * 1000;
    }

    private async Task<double> GetErrorRate(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.NextDouble() * 5; // 0-5%
    }

    private async Task<double> GetAvailabilityScore(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        return Random.Shared.NextDouble() * 5 + 95; // 95-100%
    }

    #endregion

    private void UpdateMetric(string metricName, double value, string displayName)
    {
        var metric = new MetricValue
        {
            Name = metricName,
            Value = value,
            DisplayName = displayName,
            Timestamp = DateTime.UtcNow,
            Category = GetMetricCategory(metricName)
        };

        _currentMetrics.AddOrUpdate(metricName, metric, (key, oldValue) => metric);
        
        Logger.LogTrace("Updated metric {MetricName}: {Value}", metricName, value);
    }

    private string GetMetricCategory(string metricName)
    {
        return metricName.Split('.')[0] switch
        {
            "incidents" => "Security Incidents",
            "alerts" => "Security Alerts",
            "system" => "System Health",
            "compliance" => "Compliance",
            "risk" => "Risk Management",
            "performance" => "Performance",
            _ => "General"
        };
    }

    private async Task BroadcastMetricsUpdate(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            // This would typically use SignalR to broadcast updates to connected clients
            var metricsUpdate = new DashboardMetricsUpdate
            {
                Timestamp = DateTime.UtcNow,
                Metrics = _currentMetrics.Values.ToList()
            };

            Logger.LogTrace("Broadcasting metrics update with {MetricCount} metrics", metricsUpdate.Metrics.Count);
            
            // Placeholder for SignalR hub broadcast
            // await _hubContext.Clients.Group("Dashboard").SendAsync("MetricsUpdate", metricsUpdate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error broadcasting metrics update");
        }
    }
}

/// <summary>
/// Represents a metric value
/// </summary>
public class MetricValue
{
    public required string Name { get; init; }
    public required double Value { get; init; }
    public required string DisplayName { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string Category { get; init; }
    public string? Unit { get; init; }
    public MetricType Type { get; init; } = MetricType.Gauge;
}

/// <summary>
/// Types of metrics
/// </summary>
public enum MetricType
{
    Gauge,      // Point-in-time value
    Counter,    // Cumulative value
    Rate,       // Rate per time unit
    Percentage  // Percentage value
}

/// <summary>
/// Dashboard metrics update payload
/// </summary>
public class DashboardMetricsUpdate
{
    public required DateTime Timestamp { get; init; }
    public required List<MetricValue> Metrics { get; init; }
}