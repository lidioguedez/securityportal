using SecurityPortal.Domain.DomainEvents;

namespace SecurityPortal.Domain.Services;

public interface IDomainEventDispatcher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}