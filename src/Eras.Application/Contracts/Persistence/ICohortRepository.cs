using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface ICohortRepository
    {
        Task<Cohort?> GetByNameAsync(string name);
        Task<Cohort?> GetByCourseCodeAsync(string name);
    }
}