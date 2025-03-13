using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IEvaluationRepository: IBaseRepository<Evaluation>
    {
        Task<Evaluation?> GetByNameAsync(string Name);
        new Task<List<Evaluation>> GetAllAsync();
    }
}
