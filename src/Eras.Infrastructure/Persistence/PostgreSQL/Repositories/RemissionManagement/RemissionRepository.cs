
using Eras.Application.Contracts.Persistence.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

using Microsoft.EntityFrameworkCore;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.RemissionManagement;

public sealed class RemissionRepository(AppDbContext context)
        : BaseRepository<Remission>(context),
      IRemissionRepository
{
    public async Task<IEnumerable<Remission>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Set<Remission>()
            .Where(x => x.StudentIds.Contains(studentId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Remission>> GetByStatusAsync(RemissionStatus status)
    {
        return await _context.Set<Remission>()
            .Where(x => x.Status == status)
            .ToListAsync();
    }
}