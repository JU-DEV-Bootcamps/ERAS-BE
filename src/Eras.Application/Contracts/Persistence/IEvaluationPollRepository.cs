
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IEvaluationPollRepository: IBaseRepository<Evaluation>
    {
        Task<bool> ExistsAsync(int EvaluationId, int PollId);
    }
}
