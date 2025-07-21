using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.DTOs;

namespace SecurityPortal.Application.Features.Incidents.Queries.GetIncidentById;

public class GetIncidentByIdQuery : IQuery<IncidentDto?>
{
    public Guid IncidentId { get; set; }
}