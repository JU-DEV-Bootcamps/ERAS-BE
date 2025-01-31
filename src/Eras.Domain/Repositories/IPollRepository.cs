using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IPollRepository
    {
        Task<Poll> GetTaskByName(string name);
    }
}