using AutoMapper;
using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.DTOs;
using SecurityPortal.Domain.Repositories;

namespace SecurityPortal.Application.Features.Incidents.Queries.GetIncidentById;

public class GetIncidentByIdQueryHandler : IQueryHandler<GetIncidentByIdQuery, IncidentDto?>
{
    private readonly ISecurityIncidentRepository _incidentRepository;
    private readonly IMapper _mapper;

    public GetIncidentByIdQueryHandler(
        ISecurityIncidentRepository incidentRepository,
        IMapper mapper)
    {
        _incidentRepository = incidentRepository;
        _mapper = mapper;
    }

    public async Task<IncidentDto?> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
    {
        var incident = await _incidentRepository.GetByIdWithDetailsAsync(request.IncidentId);
        
        return incident == null ? null : _mapper.Map<IncidentDto>(incident);
    }
}