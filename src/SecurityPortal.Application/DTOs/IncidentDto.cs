using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Application.DTOs;

public class IncidentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentType Type { get; set; }
    public IncidentStatus Status { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime? ReportedAt { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public Guid ZoneId { get; set; }
    public Guid PlantId { get; set; }
    public string? ZoneName { get; set; }
    public string? PlantName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    
    public List<IncidentCommentDto> Comments { get; set; } = new();
    public List<IncidentAttachmentDto> Attachments { get; set; } = new();
}

public class IncidentCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid IncidentId { get; set; }
}

public class IncidentAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public Guid IncidentId { get; set; }
}

public class IncidentSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public IncidentType Type { get; set; }
    public IncidentStatus Status { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string? AssignedTo { get; set; }
    public string? ZoneName { get; set; }
    public string? PlantName { get; set; }
}