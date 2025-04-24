using Eras.Application.DTOs.Poll;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollCohortRepository : IBaseRepository<Poll>
    {
        Task<List<Poll>> GetPollsByCohortIdAsync(int CohortId);
        Task<List<PollVariableDto>> GetPollVariablesAsync(int PollId, int CohortId);
    }
}
