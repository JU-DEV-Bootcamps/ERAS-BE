
using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class InterventionAreaValidator : AbstractValidator<InterventionArea>
{
    public InterventionAreaValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Intervention area is required.");
    }
}