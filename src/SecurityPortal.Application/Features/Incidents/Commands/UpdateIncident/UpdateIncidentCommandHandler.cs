using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.Common.Models;
using SecurityPortal.Domain.Repositories;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Application.Features.Incidents.Commands.UpdateIncident;

public class UpdateIncidentCommandHandler : ICommandHandler<UpdateIncidentCommand, Result>
{
    private readonly ISecurityIncidentRepository _incidentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncidentCommandHandler(
        ISecurityIncidentRepository incidentRepository,
        IUnitOfWork unitOfWork)
    {
        _incidentRepository = incidentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncidentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
            if (incident == null)
                return Result.Failure("Incident not found");

            incident.UpdateStatus(request.Status, request.UpdatedBy);

            if (!string.IsNullOrEmpty(request.Priority))
            {
                if (Priority.TryParse(request.Priority, out var priority))
                    incident.UpdatePriority(priority, request.UpdatedBy);
                else
                    return Result.Failure("Invalid priority value");
            }

            if (!string.IsNullOrEmpty(request.AssignedTo))
            {
                incident.AssignTo(request.AssignedTo, request.UpdatedBy);
            }

            _incidentRepository.Update(incident);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error updating incident: {ex.Message}");
        }
    }
}