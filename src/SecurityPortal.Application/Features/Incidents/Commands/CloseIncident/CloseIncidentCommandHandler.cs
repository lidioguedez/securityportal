using SecurityPortal.Application.Common.Interfaces;
using SecurityPortal.Application.Common.Models;
using SecurityPortal.Domain.Repositories;

namespace SecurityPortal.Application.Features.Incidents.Commands.CloseIncident;

public class CloseIncidentCommandHandler : ICommandHandler<CloseIncidentCommand, Result>
{
    private readonly ISecurityIncidentRepository _incidentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseIncidentCommandHandler(
        ISecurityIncidentRepository incidentRepository,
        IUnitOfWork unitOfWork)
    {
        _incidentRepository = incidentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CloseIncidentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(request.IncidentId);
            if (incident == null)
                return Result.Failure("Incident not found");

            if (!string.IsNullOrEmpty(request.ClosureNotes))
            {
                incident.AddComment(request.ClosureNotes, request.ClosedBy);
            }

            incident.Close(request.ClosedBy);

            _incidentRepository.Update(incident);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error closing incident: {ex.Message}");
        }
    }
}