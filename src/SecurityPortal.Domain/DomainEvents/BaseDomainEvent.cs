namespace SecurityPortal.Domain.DomainEvents;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; private init; } = DateTime.UtcNow;
    public Guid EventId { get; private init; } = Guid.NewGuid();
}