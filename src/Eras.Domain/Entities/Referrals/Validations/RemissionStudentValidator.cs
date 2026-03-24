using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class RemissionStudentValidator : AbstractValidator<RemissionStudent>
{
    public RemissionStudentValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.StudentId).NotNull();
    }
}
