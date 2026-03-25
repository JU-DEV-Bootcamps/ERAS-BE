
using FluentValidation;

namespace Eras.Domain.Entities.RemissionsManagement.Validators;

public sealed class RemissionValidator : AbstractValidator<Remission>
{
    public RemissionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CreatedAtUtc)
            .NotEmpty();

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Service)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AssignedProfessional)
            .MaximumLength(200);

        RuleFor(x => x.StudentIds)
            .NotNull()
            .Must(x => x.Count > 0)
            .WithMessage("At least one student must be assigned to the remission.");

        RuleForEach(x => x.StudentIds)
            .NotEmpty();

        RuleFor(x => x.Diagnosis)
            .MaximumLength(4000);

        RuleFor(x => x.Objective)
            .MaximumLength(4000);

        RuleFor(x => x.Comments)
            .MaximumLength(4000);

        RuleFor(x => x.Status)
            .IsInEnum();

        When(x => x.Plan is not null, () =>
        {
            RuleFor(x => x.Plan!)
                .SetValidator(new InterventionPlanValidator());
        });

        RuleForEach(x => x.Interventions)
            .SetInheritanceValidator(v =>
            {
                v.Add(new IndividualInterventionValidator());
                v.Add(new GroupInterventionValidator());
            });
    }
}