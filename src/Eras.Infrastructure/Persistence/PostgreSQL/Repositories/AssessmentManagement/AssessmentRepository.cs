using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

public sealed class AssessmentRepository(AppDbContext context, ILogger<AssessmentRepository> logger)
        : BaseRepository<Assessment>(context),
      IAssessmentRepository
{
    public async Task<IEnumerable<Assessment>> GetAllAsync()
    {
        return await _context.Set<Assessment>()
            .Include(a => a.Interventions)
            .ToListAsync();
    }

    public async Task<IEnumerable<Assessment>> GetByStudentIdAsync(int studentId)
    {
        return await _context.Set<Assessment>().Where(o => o != null && o.StudentIds != null && o.StudentIds.Contains(studentId)).ToListAsync();
    }

    public async Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status)
    {
        return await _context.Set<Assessment>()
            .Where(x => x.Status == status)
            .ToListAsync();
    }

    public async Task<Assessment?> GetByIdWithInterventionsAsync(int id)
    {
        return await _context.Set<Assessment>()
            .AsNoTracking()
            .Include(a => a.Interventions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAssessmentAsync(int assessmentId)
    {
        Assessment? assessment = await _context.Set<Assessment>()
            .FirstOrDefaultAsync(i => i.Id == assessmentId && i.Status != AssessmentStatus.Remitted);

        if (assessment is null)
            throw new KeyNotFoundException(
                $"Assessment '{assessmentId}' not found or not permitted.");

        var interventions = await _context.Interventions
            .Where(i => EF.Property<int?>(i, "remission_id") == assessmentId)
            .ToListAsync();

        _context.Interventions.RemoveRange(interventions);

        _context.Set<Assessment>().Remove(assessment);
        await _context.SaveChangesAsync();
    }

    public async Task<Intervention> AddInterventionAsync(int assessmentId, Intervention intervention)
    {
        _context.Set<Intervention>().Add(intervention);
        _context.Entry(intervention).Property("remission_id").CurrentValue = assessmentId;
        await _context.SaveChangesAsync();
        return intervention;
    }

    public async Task<IReadOnlyCollection<Intervention>> ReplaceInterventionsAsync(
        int assessmentId,
        IReadOnlyCollection<Intervention> interventions)
    {
        var existing = await _context.Set<Intervention>()
            .Where(i => EF.Property<int>(i, "remission_id") == assessmentId)
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

    public async Task DeleteInterventionAsync(int assessmentId, int interventionId)
    {
        Intervention? intervention = await _context.Set<Intervention>()
            .FirstOrDefaultAsync(i =>
                i.Id == interventionId &&
                EF.Property<int?>(i, "remission_id") == assessmentId);

        if (intervention is null)
            throw new KeyNotFoundException(
                $"Intervention '{interventionId}' not found for assessment '{assessmentId}'.");

        _context.Set<Intervention>().Remove(intervention);
        await _context.SaveChangesAsync();
    }

    public async Task<Intervention?> GetInterventionByIdAsync(int interventionId)
    {
        return await _context.Set<Intervention>()
            .FirstOrDefaultAsync(i => i.Id == interventionId);
    }

    public async Task AddAttachmentsAsync(int interventionId, IReadOnlyCollection<string> paths, IReadOnlyCollection<string> hashes)
    {
        Intervention? intervention = await _context.Set<Intervention>()
            .FirstOrDefaultAsync(i => i.Id == interventionId);

        if (intervention is null)
            throw new KeyNotFoundException($"Intervention '{interventionId}' not found.");

        List<string> updatedPaths = [.. intervention.Attachments, .. paths];
        List<string> updatedHashes = [.. intervention.AttachmentHashes, .. hashes];

        _context.Entry(intervention)
            .Property(i => i.Attachments)
            .CurrentValue = updatedPaths.AsReadOnly();

        _context.Entry(intervention)
            .Property(i => i.AttachmentHashes)
            .CurrentValue = updatedHashes.AsReadOnly();

        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<string>> GetAttachmentHashesAsync(int interventionId, CancellationToken cancellationToken)
    {
        Intervention? intervention = await _context.Set<Intervention>()
            .FirstOrDefaultAsync(i => i.Id == interventionId, cancellationToken);

        return intervention?.AttachmentHashes ?? Array.Empty<string>();
    }


    public async Task RemoveAttachmentAsync(int interventionId, string relativePath)
    {
        Intervention? intervention = await _context.Set<Intervention>()
            .FirstOrDefaultAsync(i => i.Id == interventionId);

        if (intervention is null)
            throw new KeyNotFoundException($"Intervention '{interventionId}' not found.");

        List<string> updated = intervention.Attachments
            .Where(a => !a.EndsWith(relativePath, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _context.Entry(intervention)
            .Property(i => i.Attachments)
            .CurrentValue = updated.AsReadOnly();

        await _context.SaveChangesAsync();
    }
}