using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
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
    }
}
