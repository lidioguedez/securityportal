using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.DTOs;
using SecurityPortal.Domain.Entities;

namespace SecurityPortal.Application.Features.Incidents.Queries.GetIncidents;

public class GetIncidentsQuery : IQuery<List<IncidentSummaryDto>>
{
    public IncidentStatus? Status { get; set; }
    public Guid? PlantId { get; set; }
    public Guid? ZoneId { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}