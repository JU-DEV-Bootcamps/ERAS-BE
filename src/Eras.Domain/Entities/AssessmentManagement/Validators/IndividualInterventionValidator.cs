
using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class IndividualInterventionValidator : AbstractValidator<IndividualIntervention>
{
    public IndividualInterventionValidator()
    {
        Include(new InterventionValidator());
    }
}