using Eras.Domain.Entities.AssessmentManagement.StatusManagement;

using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class StatusTransitionValidator<TStatus> : AbstractValidator<StatusTransitionRequest<TStatus>>
    where TStatus : struct, Enum
{
    public StatusTransitionValidator()
    {
        RuleFor(x => x)
            .Must(x => StatusTransitions<TStatus>.CanTransition(x.CurrentStatus, x.NewStatus))
            .WithMessage(x => $"Cannot transition assessment from '{x.CurrentStatus}' to '{x.NewStatus}'.");
    }
}
