using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IImportJobItemRepository : IBaseRepository<ImportJobItem>
    {
        Task<List<ImportJobItem>> GetByJobIdAsync(int ImportJobId);
        Task<List<ImportJobItem>> GetByJobIdAndStatusAsync(int ImportJobId, ImportJobStatus Status);
        Task<List<ImportJobItem>> GetByIdsAsync(int ImportJobId, List<int> ItemIds);
        Task SetStatusAsync(int Id, ImportJobStatus Status, string? ErrorMessage, DateTime UpdatedAtUtc);
        Task<int> RequeueFailedAsync(int ImportJobId, List<int> ItemIds, DateTime UpdatedAtUtc);
        /// <summary>Counts only import-phase items: Pending = Queued + Running, plus Completed and Failed (Extracted/Skipped excluded).</summary>
        Task<(int Pending, int Completed, int Failed)> GetImportPhaseCountsAsync(int ImportJobId);
        /// <summary>Marks the selected Extracted items as Queued and the rest of the Extracted items as Skipped.</summary>
        Task ConfirmSelectionAsync(int ImportJobId, List<int> SelectedIds, DateTime UpdatedAtUtc);
    }
}
