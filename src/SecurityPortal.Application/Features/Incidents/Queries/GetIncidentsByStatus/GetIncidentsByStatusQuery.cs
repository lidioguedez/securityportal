using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.DTOs;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Application.Features.Incidents.Queries.GetIncidentsByStatus;

public class GetIncidentsByStatusQuery : IQuery<List<IncidentSummaryDto>>
{
    public IncidentStatus Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}