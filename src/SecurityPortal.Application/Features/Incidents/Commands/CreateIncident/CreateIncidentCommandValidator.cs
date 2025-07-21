using FluentValidation;
using SecurityPortal.Domain.Entities;
using SecurityPortal.Domain.ValueObjects;

namespace SecurityPortal.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid incident type");

        RuleFor(x => x.Priority)
            .Must(BeValidPriority).WithMessage("Priority must be Low, Medium, High, or Critical");

        RuleFor(x => x.OccurredAt)
            .NotEmpty().WithMessage("Occurred date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Occurred date cannot be in the future");

        RuleFor(x => x.ReportedBy)
            .NotEmpty().WithMessage("Reporter is required")
            .MaximumLength(100).WithMessage("Reporter name cannot exceed 100 characters");

        RuleFor(x => x.ZoneId)
            .NotEmpty().WithMessage("Zone ID is required");

        RuleFor(x => x.PlantId)
            .NotEmpty().WithMessage("Plant ID is required");
    }

    private static bool BeValidPriority(string priority)
    {
        return Priority.TryParse(priority, out _);
    }
}