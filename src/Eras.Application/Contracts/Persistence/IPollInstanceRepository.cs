using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollInstanceRepository : IBaseRepository<PollInstance>
    {
        Task<PollInstance?> GetByUuidAsync(string uuid);
        Task<PollInstance?> GetByUuidAndStudentIdAsync(string uuid, int studentId);

        Task<IEnumerable<PollInstance>> GetByLastDays(int days);

        Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? cohortId, int? days);


    }
}