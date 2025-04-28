using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IPollInstanceRepository : IBaseRepository<PollInstance>
{
    Task<PollInstance?> GetByUuidAsync(string Uuid);
    Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId);

    Task<IEnumerable<PollInstance>> GetByLastDays(int Days);

    Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days);

    Task<AvgReportResponseVm> GetReportByPollCohortAsync(string PollUuid, int CohortId);
}
