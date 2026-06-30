using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IImportJobItemRepository : IBaseRepository<ImportJobItem>
    {
        Task<List<ImportJobItem>> GetByJobIdAsync(int ImportJobId);
        Task<List<ImportJobItem>> GetByJobIdAndStatusAsync(int ImportJobId, ImportJobStatus Status);
        Task<List<ImportJobItem>> GetByIdsAsync(int ImportJobId, List<int> ItemIds);
    }
}
