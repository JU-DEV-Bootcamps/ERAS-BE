using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IPollRepository : IBaseRepository<Poll>
    {
        Task<Poll?> GetByNameAsync(string name);
    }
}