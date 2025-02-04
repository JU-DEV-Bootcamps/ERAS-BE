using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IPollInstanceRepository
    {
        Task<PollInstance?> GetByUuidAsync(string uuid);
    }
}