using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ImportJobRepository(AppDbContext Context)
        : BaseRepository<ImportJob, ImportJobEntity>(Context, ImportJobMapper.ToDomain, ImportJobMapper.ToPersistence), IImportJobRepository
    {
        // Set-based updates avoid EF change-tracking conflicts when the job is updated several
        // times within a single worker scope.
        public async Task SetStatusAsync(int Id, ImportJobStatus Status, DateTime UpdatedAtUtc)
        {
            await _context.Set<ImportJobEntity>()
                .Where(Job => Job.Id == Id)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Job => Job.Status, Status)
                    .SetProperty(Job => Job.UpdatedAtUtc, UpdatedAtUtc));
        }

        public async Task SetResultAsync(int Id, ImportJobStatus Status, int ProcessedCount, string? ErrorMessage, DateTime UpdatedAtUtc)
        {
            await _context.Set<ImportJobEntity>()
                .Where(Job => Job.Id == Id)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Job => Job.Status, Status)
                    .SetProperty(Job => Job.ProcessedCount, ProcessedCount)
                    .SetProperty(Job => Job.ErrorMessage, ErrorMessage)
                    .SetProperty(Job => Job.UpdatedAtUtc, UpdatedAtUtc));
        }

        public async Task SetExtractedCountAsync(int Id, int ExtractedCount, DateTime UpdatedAtUtc)
        {
            await _context.Set<ImportJobEntity>()
                .Where(Job => Job.Id == Id)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Job => Job.ExtractedCount, ExtractedCount)
                    .SetProperty(Job => Job.UpdatedAtUtc, UpdatedAtUtc));
        }

        public async Task SetReadyAsync(int Id, int TotalCount, DateTime UpdatedAtUtc)
        {
            await _context.Set<ImportJobEntity>()
                .Where(Job => Job.Id == Id)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Job => Job.Status, ImportJobStatus.Ready)
                    .SetProperty(Job => Job.TotalCount, TotalCount)
                    .SetProperty(Job => Job.ExtractedCount, TotalCount)
                    .SetProperty(Job => Job.UpdatedAtUtc, UpdatedAtUtc));
        }

        // On confirm, the import total becomes the number of confirmed respondents (not all extracted).
        public async Task SetImportingAsync(int Id, int TotalCount, DateTime UpdatedAtUtc)
        {
            await _context.Set<ImportJobEntity>()
                .Where(Job => Job.Id == Id)
                .ExecuteUpdateAsync(S => S
                    .SetProperty(Job => Job.Status, ImportJobStatus.Importing)
                    .SetProperty(Job => Job.TotalCount, TotalCount)
                    .SetProperty(Job => Job.ProcessedCount, 0)
                    .SetProperty(Job => Job.UpdatedAtUtc, UpdatedAtUtc));
        }

        // Returns, per evaluation, the id of its most recently created import job (used by the
        // frontend to enable/disable "View Import" without an extra request per row).
        public async Task<Dictionary<int, int>> GetLatestImportJobIdsByEvaluationIdsAsync(
            IEnumerable<int> EvaluationIds)
        {
            return await _context.Set<ImportJobEntity>()
                .Where(Job => EvaluationIds.Contains(Job.EvaluationId))
                .GroupBy(Job => Job.EvaluationId)
                .Select(Group => Group.OrderByDescending(Job => Job.CreatedAtUtc).First())
                .ToDictionaryAsync(Job => Job.EvaluationId, Job => Job.Id);
        }
    }
}