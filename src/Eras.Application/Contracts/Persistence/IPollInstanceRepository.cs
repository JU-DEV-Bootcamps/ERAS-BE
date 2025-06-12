using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IPollInstanceRepository : IBaseRepository<PollInstance>
{
    Task<PollInstance?> GetByUuidAsync(string Uuid);
    Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId);

    Task<IEnumerable<PollInstance>> GetByLastDays(int Days, bool LastVersion, string PollUuid);

    Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days, bool LastVersion, string PollUuid);

    Task<AvgReportResponseVm> GetReportByPollCohortAsync(string PollUuid, List<int> CohortIds, bool LastVersion);

    new Task<PollInstance> UpdateAsync(PollInstance Entity);
    Task<CountReportResponseVm> GetCountReportByVariablesAsync(string PollUuid, List<int> CohortIds, List<int> VariableIds, bool LastVersion);
}
