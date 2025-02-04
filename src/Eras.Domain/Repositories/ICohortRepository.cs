using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface ICohortRepository
    {
        Task<Cohort?> GetByNameAsync(string name);
        Task<Cohort?> GetByCourseCodeAsync(string name);
    }
}