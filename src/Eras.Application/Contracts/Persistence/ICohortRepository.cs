using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface ICohortRepository : IBaseRepository<Cohort>
{
    Task<Cohort?> GetByNameAsync(string Name);
    Task<Cohort?> GetByCourseCodeAsync(string Name);
    Task<List<Cohort>> GetCohortsAsync();
    Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponentAsync(string PollUuid, string ComponentName, int CohortId);
    Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsAsync(string PollUuid, int CohortId);
    Task<List<Cohort>> GetCohortsByPollUuidAsync(string PollUuid);
}
