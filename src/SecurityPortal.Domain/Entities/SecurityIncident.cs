using SecurityPortal.Domain.Common;
using SecurityPortal.Domain.ValueObjects;
using SecurityPortal.Domain.DomainEvents;

namespace SecurityPortal.Domain.Entities;

public enum IncidentStatus
{
    Open = 1,
    InvestigationInProgress = 2,
    UnderReview = 3,
    Resolved = 4,
    Closed = 5
}

public enum IncidentType
{
    SafetyViolation = 1,
    EquipmentFailure = 2,
    EnvironmentalIncident = 3,
    SecurityBreach = 4,
    Fire = 5,
    ChemicalSpill = 6,
    PersonalInjury = 7,
    NearMiss = 8
}

public class SecurityIncident : BaseAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public IncidentType Type { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Priority Priority { get; private set; } = Priority.Medium;
    public DateTime OccurredAt { get; private set; }
    public DateTime? ReportedAt { get; private set; }
    public string ReportedBy { get; private set; } = string.Empty;
    public string? AssignedTo { get; private set; }
    public Guid ZoneId { get; private set; }
    public Guid PlantId { get; private set; }
    
    private readonly List<IncidentComment> _comments = new();
    public IReadOnlyCollection<IncidentComment> Comments => _comments.AsReadOnly();
    
    private readonly List<IncidentAttachment> _attachments = new();
    public IReadOnlyCollection<IncidentAttachment> Attachments => _attachments.AsReadOnly();

    private SecurityIncident() { }

    public static SecurityIncident Create(
        string title,
        string description,
        IncidentType type,
        Priority priority,
        DateTime occurredAt,
        string reportedBy,
        Guid zoneId,
        Guid plantId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Incident title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Incident description cannot be empty", nameof(description));
        if (string.IsNullOrWhiteSpace(reportedBy))
            throw new ArgumentException("ReportedBy cannot be empty", nameof(reportedBy));

        var incident = new SecurityIncident
        {
            Title = title,
            Description = description,
            Type = type,
            Status = IncidentStatus.Open,
            Priority = priority,
            OccurredAt = occurredAt,
            ReportedAt = DateTime.UtcNow,
            ReportedBy = reportedBy,
            ZoneId = zoneId,
            PlantId = plantId,
            CreatedBy = reportedBy
        };

        incident.AddDomainEvent(new IncidentCreatedEvent(incident.Id, incident.Title, incident.Priority.Level));

        return incident;
    }

    public void AssignTo(string assignee, string updatedBy)
    {
        AssignedTo = assignee;
        UpdateTimestamp(updatedBy);
        
        AddDomainEvent(new IncidentAssignedEvent(Id, assignee));
    }

    public void UpdateStatus(IncidentStatus newStatus, string updatedBy)
    {
        var oldStatus = Status;
        Status = newStatus;
        UpdateTimestamp(updatedBy);
        
        AddDomainEvent(new IncidentStatusChangedEvent(Id, oldStatus, newStatus));
    }

    public void UpdatePriority(Priority newPriority, string updatedBy)
    {
        var oldPriority = Priority;
        Priority = newPriority;
        UpdateTimestamp(updatedBy);
        
        AddDomainEvent(new IncidentPriorityChangedEvent(Id, oldPriority.Level, newPriority.Level));
    }

    public void AddComment(string content, string author)
    {
        var comment = IncidentComment.Create(content, author, Id);
        _comments.Add(comment);
        UpdateTimestamp(author);
    }

    public void AddAttachment(string fileName, string filePath, string uploadedBy)
    {
        var attachment = IncidentAttachment.Create(fileName, filePath, uploadedBy, Id);
        _attachments.Add(attachment);
        UpdateTimestamp(uploadedBy);
    }

    public void Close(string closedBy)
    {
        Status = IncidentStatus.Closed;
        UpdateTimestamp(closedBy);
        
        AddDomainEvent(new IncidentClosedEvent(Id, Title));
    }
}