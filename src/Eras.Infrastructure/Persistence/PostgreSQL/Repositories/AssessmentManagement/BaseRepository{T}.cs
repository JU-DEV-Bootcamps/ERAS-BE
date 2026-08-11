using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

[ExcludeFromCodeCoverage]
public class BaseRepository<T>(AppDbContext context) : BaseRepository<T, T>(context, x => x, x => x)
    where T: class
{
    public async Task<T?> GetByIdAsync(Guid id)
    {
        T? persistenceEntity = await _context.Set<T>().FindAsync(id);
        if (persistenceEntity is T found)
        {
            return found;
        }
        return null;
    }

    public async Task<T?> GetByIdNoTrackingAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _context.Set<T>().AsNoTracking();
        if(include is not null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }
}