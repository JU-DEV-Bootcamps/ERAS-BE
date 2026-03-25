
using Eras.Domain.Entities.RemissionsManagement;

using Microsoft.EntityFrameworkCore;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.RemissionManagement;

public sealed class StudentProfileRepository(AppDbContext context)
    : BaseRepository<StudentProfile>(context),
      IStudentProfileRepository
{
    public async Task<StudentProfile?> GetByStudentCodeAsync(string studentCode)
    {
        var entity = await _context.Set<StudentProfile>()
            .FirstOrDefaultAsync(x => x.StudentCode == studentCode);

        return entity;
    }
}
