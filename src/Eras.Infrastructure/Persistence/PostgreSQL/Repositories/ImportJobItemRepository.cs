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
    }
}
