using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Domain.DomainEvents;

public sealed class AlertTriggeredEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public string Title { get; }
    public PriorityLevel Priority { get; }
    public Guid PlantId { get; }

    public AlertTriggeredEvent(Guid alertId, string title, PriorityLevel priority, Guid plantId)
    {
        AlertId = alertId;
        Title = title;
        Priority = priority;
        PlantId = plantId;
    }
}

public sealed class AlertAcknowledgedEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public string AcknowledgedBy { get; }

    public AlertAcknowledgedEvent(Guid alertId, string acknowledgedBy)
    {
        AlertId = alertId;
        AcknowledgedBy = acknowledgedBy;
    }
}

public sealed class AlertResolvedEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public string ResolvedBy { get; }
    public string? ResolutionNotes { get; }

    public AlertResolvedEvent(Guid alertId, string resolvedBy, string? resolutionNotes)
    {
        AlertId = alertId;
        ResolvedBy = resolvedBy;
        ResolutionNotes = resolutionNotes;
    }
}

public sealed class AlertDismissedEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public string DismissedBy { get; }

    public AlertDismissedEvent(Guid alertId, string dismissedBy)
    {
        AlertId = alertId;
        DismissedBy = dismissedBy;
    }
}

public sealed class AlertEscalatedEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public PriorityLevel OldPriority { get; }
    public PriorityLevel NewPriority { get; }

    public AlertEscalatedEvent(Guid alertId, PriorityLevel oldPriority, PriorityLevel newPriority)
    {
        AlertId = alertId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}