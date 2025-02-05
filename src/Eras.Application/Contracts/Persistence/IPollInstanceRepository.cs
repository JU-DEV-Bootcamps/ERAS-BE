using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollInstanceRepository
    {
        Task<PollInstance?> GetByUuidAsync(string uuid);
    }
}