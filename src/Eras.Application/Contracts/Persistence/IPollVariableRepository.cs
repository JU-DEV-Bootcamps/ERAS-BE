using Eras.Application.DTOs.Views;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IPollVariableRepository : IBaseRepository<Variable>
{
        Task<Variable?> GetByPollIdAndVariableIdAsync(int PollId, int VariableId);

        Task<PagedResult<ErasCalculationsByPollDTO>?> GetByPollUuidVariableIdAsync(string PollUuid, int VariableId, Pagination Pagination);
        Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string PollUuid, string VariableIds);

        Task<List<Answer>> GetAnswersByPollUuidAsync(string PollUuid);
}
