using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IRemissionRepository : IBaseRepository<JURemission>
    {
        Task<JURemission?> GetBySubmitterUuidAsync(string SubmitterUuid);
    }
}
