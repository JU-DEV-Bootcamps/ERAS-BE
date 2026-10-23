using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

namespace Eras.Application.Services;

public class EvaluationStatusUpdater : IEvaluationStatusUpdater
{
   private readonly IEvaluationRepository _repository;
    
   public EvaluationStatusUpdater(IEvaluationRepository repository)
   {
       _repository = repository;
   }

   public async Task UpdateStatusAsync(int evaluationId)
   {
       var evaluation = await _repository.GetByIdAsync(evaluationId);
       if (evaluation == null) return;

       await PersistIfChangedAsync(evaluation);
   }

   public async Task UpdateStatusAsync(Evaluation evaluation)
   {
       await PersistIfChangedAsync(evaluation);
   }

   private async Task PersistIfChangedAsync(Evaluation evaluation)
   {
       var newStatus = EvaluationStatusService.ComputeStatus(evaluation).ToString();
       if (evaluation.Status == newStatus) return;

       evaluation.Status = newStatus;
       evaluation.Audit.ModifiedAt = DateTime.UtcNow;
       await _repository.UpdateStatusAsync(evaluation.Id, newStatus);
   }
}
