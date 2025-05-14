using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollVariableRepository : IBaseRepository<Variable>
    {
        Task<Variable?> GetByPollIdAndVariableIdAsync(int PollId, int VariableId);
        Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string PollUuid, int VariableId);
        Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string PollUuid, string VariableIds);
        Task<List<Answer>> GetSummaryByPollUuidAsync(string PollUuid);
    }
}
