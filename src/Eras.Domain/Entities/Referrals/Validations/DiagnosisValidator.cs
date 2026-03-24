using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class DiagnosisValidator : AbstractValidator<Diagnosis>
{
    public DiagnosisValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Diagnosis is required.");
    }
}
