using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface ICohortRepository : IBaseRepository<Cohort>
    {
        Task<Cohort?> GetByNameAsync(string name);
        Task<Cohort?> GetByCourseCodeAsync(string name);
        Task<List<Cohort>> GetCohortsAsync();
    }
}