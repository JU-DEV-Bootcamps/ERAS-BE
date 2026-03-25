
using FluentValidation;

namespace Eras.Domain.Entities.RemissionsManagement.Validators;

public sealed class InterventionPlanValidator : AbstractValidator<InterventionPlan>
{
    public InterventionPlanValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.SessionsPerWeek)
            .GreaterThan(0)
            .When(x => x.SessionsPerWeek.HasValue);

        RuleFor(x => x.ScheduleNotes)
            .MaximumLength(2000);
    }
}