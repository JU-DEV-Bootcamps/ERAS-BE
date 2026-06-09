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
        Task<List<Evaluation>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Evaluation>> GetExpiredWithPendingStatusAsync(IEnumerable<string> status, DateTime endDateBefore);
        Task UpdateStatusAsync(int evaluationId, string status);
    }
}
