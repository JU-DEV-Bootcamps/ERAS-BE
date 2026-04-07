
using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class GroupInterventionValidator : AbstractValidator<GroupIntervention>
{
    public GroupInterventionValidator()
    {
        Include(new InterventionValidator());

        RuleFor(x => x.Area)
            .MaximumLength(200);

        RuleForEach(x => x.ParticipantIds)
            .NotEmpty();
    }
}