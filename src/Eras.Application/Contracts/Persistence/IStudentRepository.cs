using Eras.Application.DTOs.HeatMap;
using Eras.Application.DTOs.Student;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByNameAsync(string Name);
        Task<Student?> GetByUuidAsync(string Uuid);
        Task<Student?> GetByEmailAsync(string Email);
        new Task<int> CountAsync();

        Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByComponent(
            string ComponentName,
            int Limit
        );

        Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByCohort(
            string CohortId,
            int Limit
        );

        Task<(IEnumerable<Student> Students, int TotalCount)> GetAllStudentsByPollUuidAndDaysQuery(
            int Page,
            int PageSize,
            string PollUuid,
            int? Days
        );

        Task<PagedResult<StudentAverageRiskDto>> GetStudentAverageRiskByCohortsAsync(
            Pagination Pagination,
            List<int> CohortIds,
            string PollUuid
        );

        Task<IEnumerable<Student>> GetPagedAsyncWithJoins(int Page, int PageSize);
    }
}
