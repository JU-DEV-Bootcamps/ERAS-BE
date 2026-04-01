using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

public sealed class AssessmentRepository(AppDbContext context, ILogger<AssessmentRepository> logger)
        : BaseRepository<Assessment>(context),
      IAssessmentRepository
{
    public async Task<IEnumerable<Assessment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Set<Assessment>().Where(o => o != null && o.StudentIds != null && o.StudentIds.Contains(studentId)).ToListAsync();
    }

    public async Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status)
    {
        return await _context.Set<Assessment>()
            .Where(x => x.Status == status)
            .ToListAsync();
    }
}