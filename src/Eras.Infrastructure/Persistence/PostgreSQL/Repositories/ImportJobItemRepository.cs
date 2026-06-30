using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ImportJobItemRepository(AppDbContext Context)
        : BaseRepository<ImportJobItem, ImportJobItemEntity>(Context, ImportJobItemMapper.ToDomain, ImportJobItemMapper.ToPersistence), IImportJobItemRepository
    {
        public async Task<List<ImportJobItem>> GetByJobIdAsync(int ImportJobId)
        {
            return await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId)
                .OrderBy(Item => Item.Id)
                .Select(Item => Item.ToDomain())
                .ToListAsync();
        }

        public async Task<List<ImportJobItem>> GetByJobIdAndStatusAsync(int ImportJobId, ImportJobStatus Status)
        {
            return await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId && Item.Status == Status)
                .OrderBy(Item => Item.Id)
                .Select(Item => Item.ToDomain())
                .ToListAsync();
        }

        public async Task<List<ImportJobItem>> GetByIdsAsync(int ImportJobId, List<int> ItemIds)
        {
            return await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId && ItemIds.Contains(Item.Id))
                .Select(Item => Item.ToDomain())
                .ToListAsync();
        }

        public async Task SetStatusAsync(int Id, ImportJobStatus Status, string? ErrorMessage, DateTime UpdatedAtUtc)
        {
            await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.Id == Id)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Item => Item.Status, Status)
                    .SetProperty(Item => Item.ErrorMessage, ErrorMessage)
                    .SetProperty(Item => Item.UpdatedAtUtc, UpdatedAtUtc));
        }

        // Counts items by status directly in the database (robust against in-memory/race issues).
        public async Task<(int Pending, int Completed, int Failed)> GetImportPhaseCountsAsync(int ImportJobId)
        {
            var rows = await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId)
                .GroupBy(Item => Item.Status)
                .Select(Group => new { Status = Group.Key, Count = Group.Count() })
                .ToListAsync();

            int pending = rows
                .Where(R => R.Status == ImportJobStatus.Queued || R.Status == ImportJobStatus.Running)
                .Sum(R => R.Count);
            int completed = rows.Where(R => R.Status == ImportJobStatus.Completed).Sum(R => R.Count);
            int failed = rows.Where(R => R.Status == ImportJobStatus.Failed).Sum(R => R.Count);
            return (pending, completed, failed);
        }

        public async Task ConfirmSelectionAsync(int ImportJobId, List<int> SelectedIds, DateTime UpdatedAtUtc)
        {
            // Selected extracted items → Queued (ready to import).
            await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId
                    && Item.Status == ImportJobStatus.Extracted
                    && SelectedIds.Contains(Item.Id))
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Item => Item.Status, ImportJobStatus.Queued)
                    .SetProperty(Item => Item.UpdatedAtUtc, UpdatedAtUtc));

            // Remaining extracted items → Skipped.
            await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId
                    && Item.Status == ImportJobStatus.Extracted
                    && !SelectedIds.Contains(Item.Id))
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Item => Item.Status, ImportJobStatus.Skipped)
                    .SetProperty(Item => Item.UpdatedAtUtc, UpdatedAtUtc));
        }

        // Re-queues only the still-Failed items among the given ids and bumps their retry count.
        public async Task<int> RequeueFailedAsync(int ImportJobId, List<int> ItemIds, DateTime UpdatedAtUtc)
        {
            return await _context.Set<ImportJobItemEntity>()
                .Where(Item => Item.ImportJobId == ImportJobId
                    && ItemIds.Contains(Item.Id)
                    && Item.Status == ImportJobStatus.Failed)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Item => Item.Status, ImportJobStatus.Queued)
                    .SetProperty(Item => Item.RetryCount, Item => Item.RetryCount + 1)
                    .SetProperty(Item => Item.ErrorMessage, (string?)null)
                    .SetProperty(Item => Item.UpdatedAtUtc, UpdatedAtUtc));
        }
    }
}
