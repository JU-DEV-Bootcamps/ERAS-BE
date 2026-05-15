using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class GroupInterventionValidator : AbstractValidator<GroupIntervention>
{
    public GroupInterventionValidator()
    {
        Include(new InterventionValidator());

        RuleForEach(x => x.StudentIds)
            .NotEmpty();
    }
}