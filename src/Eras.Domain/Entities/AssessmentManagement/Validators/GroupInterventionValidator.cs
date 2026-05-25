using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class GroupInterventionValidator : AbstractValidator<GroupIntervention>
{
    public GroupInterventionValidator()
    {
        Include(new InterventionValidator());

        RuleFor(x => x.StudentIds)
            .Must(ids => ids != null && ids.Any())
            .WithMessage("A group intervention must have at least one student.");

        RuleForEach(x => x.StudentIds)
            .GreaterThan(0)
            .WithMessage("Each student ID must be a valid positive integer.");
    }
}