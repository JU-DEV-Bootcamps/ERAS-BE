using Eras.Domain.Entities.AssessmentManagement;

using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class InterventionValidator : AbstractValidator<Intervention>
{
    public InterventionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.DateUtc)
            .NotEmpty();

        RuleFor(x => x.ActivityType)
            .MaximumLength(200);

        RuleFor(x => x.Professional)
            .MaximumLength(200);

        RuleFor(x => x.Comments)
            .MaximumLength(4000);

        RuleForEach(x => x.Attachments)
            .NotEmpty()
            .MaximumLength(1000);
    }
}