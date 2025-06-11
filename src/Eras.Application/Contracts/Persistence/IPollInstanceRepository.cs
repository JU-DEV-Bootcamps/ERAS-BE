using Eras.Application.Models.Consolidator;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IPollInstanceRepository : IBaseRepository<PollInstance>
{
    Task<PollInstance?> GetByUuidAsync(string Uuid);
    Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId);

    Task<IEnumerable<PollInstance>> GetByLastDays(int Days);

    Task<PagedResult<PollInstance>> GetByCohortIdAndLastDays(
            int Page,
            int PageSize,
            int[] CohortId,
            int? Days
    );

    Task<AvgReportResponseVm> GetReportByPollCohortAsync(string PollUuid, List<int> CohortIds);

    new Task<PollInstance> UpdateAsync(PollInstance Entity);
    Task<CountReportResponseVm> GetCountReportByVariablesAsync(string PollUuid, List<int> CohortIds, List<int> VariableIds);
}
