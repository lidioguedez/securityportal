using FluentValidation;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Application.Features.Incidents.Commands.UpdateIncident;

public class UpdateIncidentCommandValidator : AbstractValidator<UpdateIncidentCommand>
{
    public UpdateIncidentCommandValidator()
    {
        RuleFor(x => x.IncidentId)
            .NotEmpty().WithMessage("Incident ID is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid incident status");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority).When(x => !string.IsNullOrEmpty(x.Priority))
            .WithMessage("Priority must be Low, Medium, High, or Critical");

        RuleFor(x => x.AssignedTo)
            .MaximumLength(100).WithMessage("Assignee name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.AssignedTo));

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required")
            .MaximumLength(100).WithMessage("UpdatedBy cannot exceed 100 characters");
    }

    private static bool BeValidPriority(string? priority)
    {
        return string.IsNullOrEmpty(priority) || Priority.TryParse(priority, out _);
    }
}