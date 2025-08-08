using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface ICohortRepository : IBaseRepository<Cohort>
{
    Task<Cohort?> GetByNameAsync(string Name);
    Task<Cohort?> GetByCourseCodeAsync(string Name);
    Task<List<Cohort>> GetCohortsAsync();
    Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponentAsync(string PollUuid, string ComponentName, int CohortId, bool LastVersion);
    Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsAsync(string PollUuid, int CohortId, bool LastVersion);
    Task<List<Cohort>> GetCohortsByPollUuidAsync(string PollUuid, bool LastVersion);
    Task<List<Cohort>> GetCohortsByPollIdAsync(int PollId);
}
