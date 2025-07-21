using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.DomainEvents;

public sealed class IncidentCreatedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public string Title { get; }
    public PriorityLevel Priority { get; }

    public IncidentCreatedEvent(Guid incidentId, string title, PriorityLevel priority)
    {
        IncidentId = incidentId;
        Title = title;
        Priority = priority;
    }
}

public sealed class IncidentAssignedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public string AssignedTo { get; }

    public IncidentAssignedEvent(Guid incidentId, string assignedTo)
    {
        IncidentId = incidentId;
        AssignedTo = assignedTo;
    }
}

public sealed class IncidentStatusChangedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public Entities.IncidentStatus OldStatus { get; }
    public Entities.IncidentStatus NewStatus { get; }

    public IncidentStatusChangedEvent(Guid incidentId, Entities.IncidentStatus oldStatus, Entities.IncidentStatus newStatus)
    {
        IncidentId = incidentId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public sealed class IncidentPriorityChangedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public PriorityLevel OldPriority { get; }
    public PriorityLevel NewPriority { get; }

    public IncidentPriorityChangedEvent(Guid incidentId, PriorityLevel oldPriority, PriorityLevel newPriority)
    {
        IncidentId = incidentId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}

public sealed class IncidentClosedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public string Title { get; }

    public IncidentClosedEvent(Guid incidentId, string title)
    {
        IncidentId = incidentId;
        Title = title;
    }
}