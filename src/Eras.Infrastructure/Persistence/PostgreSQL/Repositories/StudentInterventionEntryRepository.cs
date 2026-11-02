using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class StudentInterventionEntryRepository(AppDbContext Context)
    : BaseRepository<StudentInterventionEntry, StudentInterventionEntry>(Context, Entity => Entity, Entity => Entity),
      IStudentInterventionEntryRepository
{
    public new async Task<StudentInterventionEntry?> GetByIdAsync(int Id)
    {
        return await _context.StudentInterventionEntries.FirstOrDefaultAsync(Entry => Entry.Id == Id);
    }

    public async Task<IReadOnlyList<StudentInterventionEntry>> GetByStudentIdAsync(
        int StudentId,
        CancellationToken CancellationToken = default)
    {
        return await _context.StudentInterventionEntries
            .AsNoTracking()
            .Where(Entry => Entry.StudentId == StudentId)
            .OrderByDescending(Entry => Entry.EntryDate)
            .ThenByDescending(Entry => Entry.Id)
            .ToListAsync(CancellationToken);
    }

    public async Task SoftDeleteAsync(int EntryId, string ModifiedBy)
    {
        StudentInterventionEntry entry = await _context.StudentInterventionEntries
            .FirstAsync(Candidate => Candidate.Id == EntryId);
        entry.IsDeleted = true;
        entry.Audit.ModifiedBy = ModifiedBy;
        entry.Audit.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
