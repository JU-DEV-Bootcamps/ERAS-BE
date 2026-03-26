
using FluentValidation;

namespace Eras.Domain.Entities.RemissionsManagement.Validators;

public sealed class IndividualInterventionValidator : AbstractValidator<IndividualIntervention>
{
    public IndividualInterventionValidator()
    {
        Include(new InterventionValidator());
    }
}