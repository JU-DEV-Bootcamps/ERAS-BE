using Eras.Domain.Entities;

namespace Eras.Domain.Tests
{
    public interface IPollRepositorySample
    {
        Task<List<Poll>> GetAllPollsAsync();
    }
}
