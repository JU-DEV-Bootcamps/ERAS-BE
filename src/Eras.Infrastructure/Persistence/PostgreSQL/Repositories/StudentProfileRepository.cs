using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class StudentProfileRepository(AppDbContext Context)
    : BaseRepository<StudentProfile, StudentProfile>(Context, Entity => Entity, Entity => Entity),
      IStudentProfileRepository
{
    public async Task<StudentProfile?> GetByStudentIdAsync(int StudentId)
    {
        return await _context.StudentProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(Profile => Profile.StudentId == StudentId);
    }

    public async Task<HashSet<int>> GetStudentIdsWithProfileAsync(IReadOnlyCollection<int> StudentIds)
    {
        List<int> ids = await _context.StudentProfiles
            .AsNoTracking()
            .Where(Profile => StudentIds.Contains(Profile.StudentId))
            .Select(Profile => Profile.StudentId)
            .ToListAsync();
        return ids.ToHashSet();
    }

    public async Task<bool> ExistsByIdPassportNumberAsync(string IdPassportNumber, int? ExcludeStudentId = null)
    {
        return await _context.StudentProfiles.AnyAsync(Profile =>
            Profile.IdPassportNumber == IdPassportNumber
            && (ExcludeStudentId == null || Profile.StudentId != ExcludeStudentId));
    }
}
