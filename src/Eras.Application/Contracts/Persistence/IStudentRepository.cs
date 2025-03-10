using Eras.Application.DTOs.HeatMap;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByNameAsync(string name);
        Task<Student?> GetByUuidAsync(string uuid);
        Task<Student?> GetByEmailAsync(string email);
        Task<int> CountAsync();

        Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByComponent(
            string componentName,
            int limit
        );

        Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByCohort(
            string cohortId,
            int limit
        );

        Task<(IEnumerable<Student> Students, int TotalCount)> GetAllStudentsByPollUuidAndDaysQuery(
            int page,
            int pageSize,
            string pollUuid,
            int? days
        );
    }
}
