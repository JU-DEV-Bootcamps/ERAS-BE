using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IStudentInterventionEntryRepository : IBaseRepository<StudentInterventionEntry>
{
    /// <summary>The not-deleted entry with that id, or null; unlike the base lookup it honors the soft-delete filter.</summary>
    new Task<StudentInterventionEntry?> GetByIdAsync(int Id);

    /// <summary>The not-deleted entries of a student, newest first (entry date, then the most recently created, which has the highest id).</summary>
    Task<IReadOnlyList<StudentInterventionEntry>> GetByStudentIdAsync(int StudentId, CancellationToken CancellationToken = default);

    /// <summary>Marks the entry as deleted and records who deleted it and when in its audit fields.</summary>
    Task SoftDeleteAsync(int EntryId, string ModifiedBy);
}
