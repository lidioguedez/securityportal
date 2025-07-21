using FluentValidation;

namespace SecurityPortal.Application.Features.Incidents.Commands.CloseIncident;

public class CloseIncidentCommandValidator : AbstractValidator<CloseIncidentCommand>
{
    public CloseIncidentCommandValidator()
    {
        RuleFor(x => x.IncidentId)
            .NotEmpty().WithMessage("Incident ID is required");

        RuleFor(x => x.ClosedBy)
            .NotEmpty().WithMessage("ClosedBy is required")
            .MaximumLength(100).WithMessage("ClosedBy cannot exceed 100 characters");

        RuleFor(x => x.ClosureNotes)
            .MaximumLength(1000).WithMessage("Closure notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.ClosureNotes));
    }
}