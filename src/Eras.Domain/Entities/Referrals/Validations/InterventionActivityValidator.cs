using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class InterventionActivityValidator : AbstractValidator<InterventionActivity>
{
    public InterventionActivityValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Intervention activity is required.");
    }
}
