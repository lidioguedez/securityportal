using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.Repositories;

public interface ISecurityAlertRepository
{
    Task<SecurityAlert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetByStatusAsync(AlertStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetByTypeAsync(AlertType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetRecentAsync(int hours = 24, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityAlert>> GetUnacknowledgedAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SecurityAlert alert, CancellationToken cancellationToken = default);
    Task UpdateAsync(SecurityAlert alert, CancellationToken cancellationToken = default);
    Task DeleteAsync(SecurityAlert alert, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
}