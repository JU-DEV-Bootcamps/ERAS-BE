using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IEvaluationRepository : IBaseRepository<Evaluation>
    {
        Task<Evaluation?> GetStatusById(int Id);
        Task<Evaluation?> GetByNameAsync(string Name);
        Task<Evaluation?> GetByNameForUpdateAsync(int Id, string Name);
        Task<Evaluation?> GetByIdForUpdateAsync(int Id);        
        new Task<List<Evaluation>> GetAllAsync();
    }
}
