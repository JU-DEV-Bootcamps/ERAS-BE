using Eras.Domain.Entities.AssessmentManagement;
using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class InterventionValidator : AbstractValidator<Intervention>
{
    public InterventionValidator()
    {
        RuleFor(x => x.DateUtc)
            .NotEmpty();

        RuleFor(x => x.Activity)
            .MaximumLength(200);

        RuleFor(x => x.Area)
            .MaximumLength(200);

        RuleFor(x => x.Professional)
            .MaximumLength(200);

        RuleFor(x => x.Comments)
            .MaximumLength(4000);
            
        RuleFor(x => x.StudentIds)
            .NotNull();

        RuleFor(x => x.Remarks)
            .MaximumLength(1000);

        RuleForEach(x => x.Attachments)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.RiskLevel)
             .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(5);
    }
}