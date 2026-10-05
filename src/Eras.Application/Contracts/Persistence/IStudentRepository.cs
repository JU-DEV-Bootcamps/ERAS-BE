using Eras.Application.DTOs.HeatMap;
using Eras.Application.DTOs;
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

        /// <summary>
        /// True when the student already has answers (poll instances) or appears in an assessment,
        /// i.e. deleting it would orphan history.
        /// </summary>
        Task<bool> HasRelatedDataAsync(int StudentId);

        /// <summary>
        /// Updates only the display name and email, leaving details, cohorts and answers untouched.
        /// </summary>
        Task UpdateIdentityAsync(int StudentId, string Name, string Email, string ModifiedBy);

        /// <summary>
        /// Soft-deletes the student and its profile: they are flagged and hidden everywhere
        /// (global query filter) but the rows are kept.
        /// </summary>
        Task SoftDeleteAsync(int StudentId, string ModifiedBy);
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
            string PollUuid,
            bool LastVersion,
            int? evaluationId
        );

        Task<IEnumerable<Student>> GetPagedAsyncWithJoins(int Page, int PageSize);
        Task<IEnumerable<Student>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        Task<Dictionary<int, double>> GetAverageRiskByStudentIdsAsync(IEnumerable<int> StudentIds);

        Task<IEnumerable<StudentLightDto>> GetAllLightAsync();
    }
}
