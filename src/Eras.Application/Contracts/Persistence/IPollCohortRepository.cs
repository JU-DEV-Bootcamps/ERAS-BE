using Eras.Application.DTOs.Poll;
using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollCohortRepository : IBaseRepository<Poll>
    {
        Task<List<Poll>> GetPollsByCohortIdAsync(int CohortId);
        Task<List<PollVariableDto>> GetPollVariablesAsync(int PollId, int CohortId);
        Task<List<GetCohortComponentsByPollResponse>> GetCohortComponentsByPoll(string PollUuid, bool LastVersion);
        Task<List<GetCohortStudentsRiskByPollResponse>> GetCohortStudentsRiskByPoll(string PollUuid, int CohortId);
    }
}
