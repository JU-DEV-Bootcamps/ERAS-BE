using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface ICohortRepository : IBaseRepository<Cohort>
    {
        Task<Cohort?> GetByNameAsync(string Name);
        Task<Cohort?> GetByCourseCodeAsync(string Name);
        Task<List<Cohort>> GetCohortsAsync();
        Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponent(string PollUuid, string ComponentName, int CohortId);
        Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudents(string PollUuid, int CohortId);
    }
}
