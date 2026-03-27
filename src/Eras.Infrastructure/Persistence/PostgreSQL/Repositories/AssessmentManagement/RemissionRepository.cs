using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

public sealed class RemissionRepository(AppDbContext context)
        : BaseRepository<Assessment>(context),
      IAssessmentRepository
{
    public async Task<IEnumerable<Assessment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Set<Assessment>()
            .Where(x => x.StudentIds.Contains(studentId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status)
    {
        return await _context.Set<Assessment>()
            .Where(x => x.Status == status)
            .ToListAsync();
    }
}