using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollRepository : IBaseRepository<Poll>
    {
        Task<Poll?> GetByNameAsync(string name);
    }
}