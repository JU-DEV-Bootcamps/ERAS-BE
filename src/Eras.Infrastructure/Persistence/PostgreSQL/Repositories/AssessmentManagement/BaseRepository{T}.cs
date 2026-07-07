using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

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
    public async Task<T?> GetByIdAsync(int id)
    {
        T? persistenceEntity = await _context.Set<T>().FindAsync(id);
        if (persistenceEntity is T found)
        {
            return found;
        }
        return null;
    }

    public async Task<T?> GetByIdNoTrackingAsync(int id)
    {
        T? persistenceEntity = await _context.Set<T>().FindAsync(id);
        if (persistenceEntity is T found)
        {
            _context.Entry(persistenceEntity).State = EntityState.Detached;
            return found;
        }
        return null;
    }
}
