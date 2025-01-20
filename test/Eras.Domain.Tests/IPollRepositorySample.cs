using Eras.Infrastructure.Persistence.PostgreSQL;

namespace Eras.Domain.Tests
{
    public interface IPollRepositorySample
    {
        Task<List<Polls>> GetAllPollsAsync();
    }
}
