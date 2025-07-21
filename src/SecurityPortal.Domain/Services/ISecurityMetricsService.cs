using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.Services;

public interface ISecurityMetricsService
{
    Task<int> GetActiveIncidentsCountAsync(Guid? plantId = null, CancellationToken cancellationToken = default);
    Task<int> GetActiveAlertsCountAsync(Guid? plantId = null, CancellationToken cancellationToken = default);
    Task<int> GetIncidentsByPriorityCountAsync(PriorityLevel priority, Guid? plantId = null, CancellationToken cancellationToken = default);
    Task<int> GetAlertsByPriorityCountAsync(PriorityLevel priority, Guid? plantId = null, CancellationToken cancellationToken = default);
    Task<double> GetIncidentResolutionTimeAsync(Guid? plantId = null, int days = 30, CancellationToken cancellationToken = default);
    Task<double> GetAlertResponseTimeAsync(Guid? plantId = null, int days = 30, CancellationToken cancellationToken = default);
    Task<Dictionary<IncidentType, int>> GetIncidentsByTypeAsync(Guid? plantId = null, int days = 30, CancellationToken cancellationToken = default);
    Task<Dictionary<AlertType, int>> GetAlertsByTypeAsync(Guid? plantId = null, int days = 30, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetIncidentTrendsAsync(Guid? plantId = null, int days = 30, CancellationToken cancellationToken = default);
}