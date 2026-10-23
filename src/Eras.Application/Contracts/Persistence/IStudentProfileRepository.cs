using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IStudentProfileRepository : IBaseRepository<StudentProfile>
{
    Task<StudentProfile?> GetByStudentIdAsync(int StudentId);

    /// <summary>
    /// Subset of the given student ids that have a profile (i.e. were registered through "New Student").
    /// </summary>
    Task<HashSet<int>> GetStudentIdsWithProfileAsync(IReadOnlyCollection<int> StudentIds);

    /// <summary>
    /// True when another student already uses this ID/passport number.
    /// </summary>
    Task<bool> ExistsByIdPassportNumberAsync(string IdPassportNumber, int? ExcludeStudentId = null);
}
