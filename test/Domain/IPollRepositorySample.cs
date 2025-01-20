using Infrastructure.Persistence.PostgreSQL;
namespace ERAS.Domain.Tests
{
    public interface IPollRepositorySample
    {
        Task<List<Polls>> GetAllPollsAsync();
    }
}
