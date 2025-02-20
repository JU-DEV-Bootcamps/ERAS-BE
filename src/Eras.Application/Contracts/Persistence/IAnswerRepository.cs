using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IAnswerRepository : IBaseRepository<Answer>
    {
      Task<List<Answer>?> GetByPollInstanceIdAsync(string uuid);
      Task<List<Answer>?> GetByStudentIdAsync(string uuid);
    }
}
