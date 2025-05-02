using Eras.Application.DTOs.Poll;
using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollCohortRepository : IBaseRepository<Poll>
    {
        Task<List<Poll>> GetPollsByCohortIdAsync(int cohortId);
        Task<List<PollVariableDto>> GetPollVariablesAsync(int pollId, int cohortId);
        Task<List<GetCohortComponentsByPollResponse>> GetCohortComponentsByPoll(string PollUuid);
        Task<List<GetCohortStudentsRiskByPollResponse>> GetCohortStudentsRiskByPoll(string PollUuid, int CohortId);
    }
}
