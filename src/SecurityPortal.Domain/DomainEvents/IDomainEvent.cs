using MediatR;

namespace SecurityPortal.Domain.DomainEvents;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}