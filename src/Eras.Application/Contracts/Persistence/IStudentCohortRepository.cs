using Eras.Application.Models.Response.Controllers.CohortsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentCohortRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByCohortIdAndStudentIdAsync(int CohortId, int StudentId);
        Task<IEnumerable<Student>?> GetAllStudentsByCohortIdAsync(int CohortId);
        Task<CohortSummaryResponse> GetCohortsSummaryAsync(Pagination Pagination);
    }
}
