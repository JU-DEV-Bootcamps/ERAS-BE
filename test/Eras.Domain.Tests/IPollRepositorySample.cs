using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;

namespace Eras.Domain.Tests
{
    public interface IPollRepositorySample
    {
        Task<List<Poll>> GetAllPollsAsync();
    }
}
