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

    public async Task<Assessment?> GetByIdWithInterventionsAsync(Guid id)
    {
        return await _context.Set<Assessment>()
            .AsNoTracking()
            .Include(a => a.Interventions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Intervention> AddInterventionAsync(Guid assessmentId, Intervention intervention)
    {
        _context.Set<Intervention>().Add(intervention);
        _context.Entry(intervention).Property("remission_id").CurrentValue = assessmentId;
        await _context.SaveChangesAsync();
        return intervention;
    }

    public async Task<IReadOnlyCollection<Intervention>> ReplaceInterventionsAsync(
        Guid assessmentId,
        IReadOnlyCollection<Intervention> interventions)
    {
        var existing = await _context.Set<Intervention>()
            .Where(i => EF.Property<Guid?>(i, "remission_id") == assessmentId)
            .ToListAsync();

        _context.Set<Intervention>().RemoveRange(existing);

        foreach (Intervention intervention in interventions)
        {
            _context.Set<Intervention>().Add(intervention);
            _context.Entry(intervention).Property("remission_id").CurrentValue = assessmentId;
        }

        await _context.SaveChangesAsync();
        return interventions;
    }

    public async Task DeleteInterventionAsync(Guid assessmentId, Guid interventionId)
    {
        Intervention? intervention = await _context.Set<Intervention>()
            .FirstOrDefaultAsync(i =>
                i.Id == interventionId &&
                EF.Property<Guid?>(i, "remission_id") == assessmentId);

        if (intervention is null)
            throw new KeyNotFoundException(
                $"Intervention '{interventionId}' not found for assessment '{assessmentId}'.");

        _context.Set<Intervention>().Remove(intervention);
        await _context.SaveChangesAsync();
    }
}