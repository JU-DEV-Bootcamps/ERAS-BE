using FluentValidation;

namespace Eras.Domain.Entities.UserManagement;

public sealed class ProfessionalProfileValidator : AbstractValidator<ProfessionalProfile>
{
    public ProfessionalProfileValidator()
    {
        RuleFor(Profile => Profile.Faculty)
            .NotEmpty()
            .WithMessage("Faculty is required.")
            .MaximumLength(200)
            .WithMessage("Faculty must be under 200 characters.");

        RuleFor(Profile => Profile.OfficeHours)
            .MaximumLength(200)
            .WithMessage("Office hours must be under 200 characters.");

        RuleForEach(Profile => Profile.SubjectsTaught)
            .MaximumLength(200)
            .WithMessage("Each subject must be under 200 characters.");

        RuleForEach(Profile => Profile.Courses).ChildRules(Course =>
        {
            Course.RuleFor(Entry => Entry.Code)
                .NotEmpty()
                .WithMessage("Course code is required.")
                .MaximumLength(20)
                .WithMessage("Course code must be under 20 characters.");

            Course.RuleFor(Entry => Entry.Name)
                .NotEmpty()
                .WithMessage("Course name is required.")
                .MaximumLength(200)
                .WithMessage("Course name must be under 200 characters.");
        });
    }
}
