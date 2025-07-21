namespace SecurityPortal.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IPlantRepository Plants { get; }
    ISecurityIncidentRepository SecurityIncidents { get; }
    ISecurityAlertRepository SecurityAlerts { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}