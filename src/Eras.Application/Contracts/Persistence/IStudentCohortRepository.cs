using Eras.Application.Models.Response.Controllers.CohortsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentCohortRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByCohortIdAndStudentIdAsync(int CohortId, int StudentId);
        Task<IEnumerable<Student>?> GetAllStudentsByCohortIdAsync(int CohortId);

        /// <summary>The cohort the student was most recently linked to, or null when there is none.</summary>
        Task<int?> GetCohortIdByStudentIdAsync(int StudentId);

        /// <summary>
        /// Moves the student's most recent cohort link to <paramref name="CohortId"/>, or creates
        /// the link when the student has none. Does nothing if the student is already in it.
        /// </summary>
        Task ChangeCohortAsync(int StudentId, int CohortId);
        Task<CohortSummaryResponse> GetCohortsSummaryAsync(Pagination Pagination, DateTime? startDate = null, DateTime? endDate = null);
    }
}
