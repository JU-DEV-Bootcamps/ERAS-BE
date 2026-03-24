using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class RemissionValidator : AbstractValidator<Remission>
{
    public RemissionValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.CreatedBy).NotNull();
        RuleFor(x => x.ServiceId).NotNull();
        RuleFor(x => x.Diagnosis).NotNull();
        RuleFor(x => x.Objective).NotNull();
        RuleFor(x => x.CreatedAtUtc).NotEmpty();

        RuleFor(x => x.Students)
            .Must(x => x.Count > 0)
            .WithMessage("A remission must have at least one student.");
    }
}
