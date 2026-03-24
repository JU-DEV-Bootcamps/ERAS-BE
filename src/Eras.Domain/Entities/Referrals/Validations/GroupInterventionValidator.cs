using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class GroupInterventionValidator : AbstractValidator<GroupIntervention>
{
    public GroupInterventionValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Activity).NotNull();
        RuleFor(x => x.Area).NotNull();
        RuleFor(x => x.ProfessionalId).NotNull();
        RuleFor(x => x.DateUtc).NotEmpty();

        RuleFor(x => x.Participants)
            .Must(x => x.Count > 0)
            .WithMessage("A group intervention must have at least one participant.");
    }
}
