using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.Repositories;

public interface ISecurityIncidentRepository
{
    Task<SecurityIncident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByZoneIdAsync(Guid zoneId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByAssigneeAsync(string assignee, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetRecentAsync(int days = 30, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task UpdateAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task DeleteAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default);
    Task<int> GetCountByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
}