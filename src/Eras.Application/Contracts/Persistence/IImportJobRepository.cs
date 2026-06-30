using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IImportJobRepository : IBaseRepository<ImportJob>
    {
        Task SetStatusAsync(int Id, ImportJobStatus Status, DateTime UpdatedAtUtc);
        Task SetResultAsync(int Id, ImportJobStatus Status, int ProcessedCount, string? ErrorMessage, DateTime UpdatedAtUtc);
    }
}
