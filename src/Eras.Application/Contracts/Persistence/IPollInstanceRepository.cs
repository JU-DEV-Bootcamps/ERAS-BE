using Eras.Application.Dtos;
using Eras.Application.Models.Consolidator;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IPollInstanceRepository : IBaseRepository<PollInstance>
{
    Task<PollInstance?> GetByUuidAsync(string Uuid);
    Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId);
    Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId, int EvaluationId);
    Task<bool> ExistsByPollNameAndStudentEmailAsync(string PollName, string StudentEmail);

    Task<IEnumerable<PollInstance>> GetByLastDays(int Days, bool LastVersion, string PollUuid);

    Task<PagedResult<PollInstance>> GetByCohortIdAndLastDays(
            int Page,
            int PageSize,
            int[] CohortId,
            int? Days,
            bool LastVersion,
            string PollUuid,
            DateTime? StartDate,
            DateTime? EndDate
    );

    Task<AvgReportResponseVm> GetReportByPollCohortAsync(string PollUuid, List<int> CohortIds, bool LastVersion, DateTime StartDate, DateTime EndDate);

    new Task<PollInstance> UpdateAsync(PollInstance Entity);
    Task<CountReportResponseVm> GetCountReportByVariablesAsync(string PollUuid, List<int> CohortIds, List<int> VariableIds, bool LastVersion, DateTime startDate, DateTime endDate, int? EvaluationId);
    new Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> ExistsForStudentAndEvaluationAsync(int StudentId, string PollUuid, int EvaluationId);
    Task<PollInstance?> FindMatchingSourceInstanceAsync(int studentId, int currentPollInstanceId, PollDTO incomingPoll);
    Task SetSourceInstanceAsync(int pollInstanceId, int sourceInstanceId);
}
