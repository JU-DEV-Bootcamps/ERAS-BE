using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

public sealed class RemissionRepository(AppDbContext context, ILogger<RemissionRepository> logger)
        : BaseRepository<Assessment>(context),
      IAssessmentRepository
{
    public async Task<IEnumerable<Assessment>> GetByStudentIdAsync(Guid studentId)
    {
        // aparently EF Postgres driver can't translate this correctly and this causes exception when there is no Assessment
        // That's why I'm wrapping it in a try catch

        try
        {
            return await _context.Set<Assessment>().Where(o => o != null && o.StudentIds != null && o.StudentIds.Contains(studentId)).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogInformation("No assessments found for studentId: {studentId}", studentId);
            return [];
        }
    }

    public async Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status)
    {
        return await _context.Set<Assessment>()
            .Where(x => x.Status == status)
            .ToListAsync();
    }
}