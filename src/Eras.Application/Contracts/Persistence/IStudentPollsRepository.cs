using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentPollsRepository : IBaseRepository<Poll>
    {
        Task<List<Poll>> GetPollsByStudentIdAsync(int StudentId);
    }
}
