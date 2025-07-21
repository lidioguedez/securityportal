using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.ValueObjects;
using SecurityPortal.Domain.DomainEvents;

namespace SecurityPortal.Domain.Entities;

public enum AlertType
{
    SystemAlert = 1,
    SecurityBreach = 2,
    EnvironmentalHazard = 3,
    EquipmentFailure = 4,
    SafetyViolation = 5,
    MaintenanceRequired = 6
}

public enum AlertStatus
{
    Active = 1,
    Acknowledged = 2,
    InProgress = 3,
    Resolved = 4,
    Dismissed = 5
}

public class SecurityAlert : BaseAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public AlertType Type { get; private set; }
    public AlertStatus Status { get; private set; }
    public Priority Priority { get; private set; } = Priority.Medium;
    public DateTime TriggeredAt { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public string? AcknowledgedBy { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public Guid? ZoneId { get; private set; }
    public Guid PlantId { get; private set; }
    public Guid? RelatedIncidentId { get; private set; }

    private SecurityAlert() { }

    public static SecurityAlert Create(
        string title,
        string message,
        AlertType type,
        Priority priority,
        Guid plantId,
        Guid? zoneId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Alert title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Alert message cannot be empty", nameof(message));

        var alert = new SecurityAlert
        {
            Title = title,
            Message = message,
            Type = type,
            Status = AlertStatus.Active,
            Priority = priority,
            TriggeredAt = DateTime.UtcNow,
            PlantId = plantId,
            ZoneId = zoneId,
            CreatedBy = "System"
        };

        alert.AddDomainEvent(new AlertTriggeredEvent(alert.Id, alert.Title, alert.Priority.Level, alert.PlantId));

        return alert;
    }

    public void Acknowledge(string acknowledgedBy)
    {
        if (Status == AlertStatus.Resolved || Status == AlertStatus.Dismissed)
            throw new InvalidOperationException("Cannot acknowledge a resolved or dismissed alert");

        Status = AlertStatus.Acknowledged;
        AcknowledgedAt = DateTime.UtcNow;
        AcknowledgedBy = acknowledgedBy;
        UpdateTimestamp(acknowledgedBy);

        AddDomainEvent(new AlertAcknowledgedEvent(Id, acknowledgedBy));
    }

    public void Resolve(string resolvedBy, string? resolutionNotes = null)
    {
        Status = AlertStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        UpdateTimestamp(resolvedBy);

        AddDomainEvent(new AlertResolvedEvent(Id, resolvedBy, resolutionNotes));
    }

    public void Dismiss(string dismissedBy)
    {
        Status = AlertStatus.Dismissed;
        UpdateTimestamp(dismissedBy);

        AddDomainEvent(new AlertDismissedEvent(Id, dismissedBy));
    }

    public void LinkToIncident(Guid incidentId, string updatedBy)
    {
        RelatedIncidentId = incidentId;
        UpdateTimestamp(updatedBy);
    }

    public void EscalatePriority(Priority newPriority, string updatedBy)
    {
        if (newPriority.Level <= Priority.Level)
            throw new InvalidOperationException("New priority must be higher than current priority");

        var oldPriority = Priority;
        Priority = newPriority;
        UpdateTimestamp(updatedBy);

        AddDomainEvent(new AlertEscalatedEvent(Id, oldPriority.Level, newPriority.Level));
    }
}