using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface ICohortRepository : IBaseRepository<Cohort>
    {
        Task<Cohort?> GetByNameAsync(string Name);
        Task<Cohort?> GetByCourseCodeAsync(string Name);
        Task<List<Cohort>> GetCohortsAsync();
    }
}
