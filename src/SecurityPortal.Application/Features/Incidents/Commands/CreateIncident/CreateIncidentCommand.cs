using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.Common.Models;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommand : ICommand<Result<Guid>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentType Type { get; set; }
    public string Priority { get; set; } = "Medium";
    public DateTime OccurredAt { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public Guid ZoneId { get; set; }
    public Guid PlantId { get; set; }
}