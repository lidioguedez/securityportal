using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.Common.Models;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Application.Features.Incidents.Commands.UpdateIncident;

public class UpdateIncidentCommand : ICommand<Result>
{
    public Guid IncidentId { get; set; }
    public IncidentStatus Status { get; set; }
    public string? Priority { get; set; }
    public string? AssignedTo { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}