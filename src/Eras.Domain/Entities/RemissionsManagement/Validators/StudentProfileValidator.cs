
using FluentValidation;

namespace Eras.Domain.Entities.RemissionsManagement.Validators;

public sealed class StudentProfileValidator : AbstractValidator<StudentProfile>
{
    public StudentProfileValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.StudentCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.SupportAndReferralHistory)
            .MaximumLength(4000);

        RuleFor(x => x.CharacterizationOrCurrentContext)
            .MaximumLength(4000);
    }
}
