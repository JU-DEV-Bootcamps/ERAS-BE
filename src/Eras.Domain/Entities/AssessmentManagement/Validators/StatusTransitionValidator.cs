using Eras.Domain.Entities.AssessmentManagement.StatusManagement;

using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement.Validators;

public sealed class AssessmentStatusTransitionValidator : AbstractValidator<StatusTransitionRequest<AssessmentStatus>>
{
    public AssessmentStatusTransitionValidator()
    {
        RuleFor(x => x)
            .Must(x => AssessmentStatusTransitions.CanTransition(x.CurrentStatus, x.NewStatus))
            .WithMessage(x => $"Cannot transition assessment from '{x.CurrentStatus}' to '{x.NewStatus}'.");
    }
}

public sealed class InterventionStatusTransitionValidator : AbstractValidator<StatusTransitionRequest<InterventionStatus>>
{
    public InterventionStatusTransitionValidator()
    {
        RuleFor(x => x)
            .Must(x => InterventionStatusTransitions.CanTransition(x.CurrentStatus, x.NewStatus))
            .WithMessage(x => $"Cannot transition intervention from '{x.CurrentStatus}' to '{x.NewStatus}'.");
    }
}
