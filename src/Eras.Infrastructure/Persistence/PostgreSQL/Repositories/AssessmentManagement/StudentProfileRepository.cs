using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

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
