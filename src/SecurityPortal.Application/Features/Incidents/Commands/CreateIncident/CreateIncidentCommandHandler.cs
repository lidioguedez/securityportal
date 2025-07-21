using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.Common.Models;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandHandler : ICommandHandler<CreateIncidentCommand, Result<Guid>>
{
    private readonly ISecurityIncidentRepository _incidentRepository;
    private readonly IPlantRepository _plantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncidentCommandHandler(
        ISecurityIncidentRepository incidentRepository,
        IPlantRepository plantRepository,
        IUnitOfWork unitOfWork)
    {
        _incidentRepository = incidentRepository;
        _plantRepository = plantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var plant = await _plantRepository.GetByIdAsync(request.PlantId);
            if (plant == null)
                return Result<Guid>.Failure("Plant not found");

            var zone = plant.Zones.FirstOrDefault(z => z.Id == request.ZoneId);
            if (zone == null)
                return Result<Guid>.Failure("Zone not found");

            if (!Priority.TryParse(request.Priority, out var priority))
                return Result<Guid>.Failure("Invalid priority value");

            var incident = SecurityIncident.Create(
                request.Title,
                request.Description,
                request.Type,
                priority,
                request.OccurredAt,
                request.ReportedBy,
                request.ZoneId,
                request.PlantId);

            await _incidentRepository.AddAsync(incident);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(incident.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating incident: {ex.Message}");
        }
    }
}