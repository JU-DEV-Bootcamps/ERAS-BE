using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class IndividualInterventionValidator : AbstractValidator<IndividualIntervention>
{
    public IndividualInterventionValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.StudentId).NotNull();
        RuleFor(x => x.Activity).NotNull();
        RuleFor(x => x.ProfessionalId).NotNull();
        RuleFor(x => x.DateUtc).NotEmpty();
    }
}
