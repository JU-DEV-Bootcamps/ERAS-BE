using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class ObjectiveValidator : AbstractValidator<Objective>
{
    public ObjectiveValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Objective is required.");
    }
}
