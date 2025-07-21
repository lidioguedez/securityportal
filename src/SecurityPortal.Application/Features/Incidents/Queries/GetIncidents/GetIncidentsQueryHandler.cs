using AutoMapper;
using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.DTOs;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Domain.Specifications;

namespace SecurityPortal.Application.Features.Incidents.Queries.GetIncidents;

public class GetIncidentsQueryHandler : IQueryHandler<GetIncidentsQuery, List<IncidentSummaryDto>>
{
    private readonly ISecurityIncidentRepository _incidentRepository;
    private readonly IMapper _mapper;

    public GetIncidentsQueryHandler(
        ISecurityIncidentRepository incidentRepository,
        IMapper mapper)
    {
        _incidentRepository = incidentRepository;
        _mapper = mapper;
    }

    public async Task<List<IncidentSummaryDto>> Handle(GetIncidentsQuery request, CancellationToken cancellationToken)
    {
        var specification = new IncidentFilterSpecification(
            request.Status,
            request.PlantId,
            request.ZoneId,
            request.AssignedTo,
            request.FromDate,
            request.ToDate);

        var incidents = await _incidentRepository.GetPaginatedAsync(
            specification,
            request.PageNumber,
            request.PageSize);

        return _mapper.Map<List<IncidentSummaryDto>>(incidents);
    }
}