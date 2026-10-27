using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IEvaluationRepository : IBaseRepository<Evaluation>
    {
        /// <summary>Evaluation row only (dates, status), without its polls and poll instances.</summary>
        Task<Evaluation?> GetByIdWithoutRelationsAsync(int Id);
        Task<Evaluation?> GetStatusById(int Id);
        Task<Evaluation?> GetByNameAsync(string Name);
        Task<Evaluation?> GetByNameForUpdateAsync(int Id, string Name);
        Task<Evaluation?> GetByIdForUpdateAsync(int Id);
        new Task<List<Evaluation>> GetAllAsync();
        Task<List<Evaluation>> GetByDateRange(DateTime startDate, DateTime endDate);
        new Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Evaluation>> GetExpiredWithPendingStatusAsync(IEnumerable<string> status, DateTime endDateBefore, CancellationToken ct);
        Task UpdateStatusAsync(int evaluationId, string status);
    }
}
