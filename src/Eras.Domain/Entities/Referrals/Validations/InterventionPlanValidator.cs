using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class InterventionPlanValidator : AbstractValidator<InterventionPlan>
{
    public InterventionPlanValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.StudentId).NotNull();
        RuleFor(x => x.Diagnosis).NotNull();
        RuleFor(x => x.Objective).NotNull();
    }
}
