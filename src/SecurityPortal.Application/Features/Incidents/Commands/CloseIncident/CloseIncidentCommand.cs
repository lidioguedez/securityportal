using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.Common.Models;

namespace SecurityPortal.Application.Features.Incidents.Commands.CloseIncident;

public class CloseIncidentCommand : ICommand<Result>
{
    public Guid IncidentId { get; set; }
    public string ClosedBy { get; set; } = string.Empty;
    public string? ClosureNotes { get; set; }
}