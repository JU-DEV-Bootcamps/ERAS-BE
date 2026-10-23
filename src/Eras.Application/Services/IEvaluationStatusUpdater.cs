using Eras.Domain.Entities;

namespace Eras.Application.Services;

public interface IEvaluationStatusUpdater
{
    Task UpdateStatusAsync(Evaluation evaluation);
    Task UpdateStatusAsync(int evaluationId);
}
