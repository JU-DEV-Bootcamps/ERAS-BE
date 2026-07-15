namespace Eras.Domain.Entities.AssessmentManagement.StatusManagement;

public sealed record StatusTransitionRequest<TStatus>(TStatus CurrentStatus, TStatus NewStatus)
    where TStatus : struct, Enum;
