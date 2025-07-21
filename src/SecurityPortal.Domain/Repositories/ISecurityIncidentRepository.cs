using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;
using SecurityPortal.Domain.Common;

namespace SecurityPortal.Domain.Repositories;

public interface ISecurityIncidentRepository
{
    Task<SecurityIncident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SecurityIncident?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByZoneIdAsync(Guid zoneId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetByAssigneeAsync(string assignee, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> GetRecentAsync(int days = 30, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityIncident>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<SecurityIncident>> GetPaginatedAsync<T>(T specification, int pageNumber, int pageSize, CancellationToken cancellationToken = default) where T : ISpecification<SecurityIncident>;
    Task AddAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task UpdateAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    void Update(SecurityIncident incident);
    Task DeleteAsync(SecurityIncident incident, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountByStatusAsync(IncidentStatus status, CancellationToken cancellationToken = default);
    Task<int> GetCountByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
}