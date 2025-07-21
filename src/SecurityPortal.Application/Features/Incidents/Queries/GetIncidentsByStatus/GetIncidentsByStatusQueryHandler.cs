using AutoMapper;
using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.DTOs;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Domain.Specifications;

namespace SecurityPortal.Application.Features.Incidents.Queries.GetIncidentsByStatus;

public class GetIncidentsByStatusQueryHandler : IQueryHandler<GetIncidentsByStatusQuery, List<IncidentSummaryDto>>
{
    private readonly ISecurityIncidentRepository _incidentRepository;
    private readonly IMapper _mapper;

    public GetIncidentsByStatusQueryHandler(
        ISecurityIncidentRepository incidentRepository,
        IMapper mapper)
    {
        _incidentRepository = incidentRepository;
        _mapper = mapper;
    }

    public async Task<List<IncidentSummaryDto>> Handle(GetIncidentsByStatusQuery request, CancellationToken cancellationToken)
    {
        var specification = new IncidentsByStatusSpecification(request.Status);
        
        var incidents = await _incidentRepository.GetPaginatedAsync(
            specification,
            request.PageNumber,
            request.PageSize);

        return _mapper.Map<List<IncidentSummaryDto>>(incidents);
    }
}