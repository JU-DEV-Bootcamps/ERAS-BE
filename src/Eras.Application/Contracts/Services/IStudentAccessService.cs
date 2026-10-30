namespace Eras.Application.Contracts.Services;

/// <summary>
/// Decides which students the current user may reach (ADR-0004): an Administrator or Officer reaches every
/// student, a Professional only the students of the assessments they are assigned to, a Guest none.
/// A violation is a <c>BussinessException</c> with status 403.
/// </summary>
public interface IStudentAccessService
{
    /// <summary>True when the current user has the Administrator role.</summary>
    bool IsAdministrator { get; }

    /// <summary>
    /// The ids of the students the current user may reach, or <c>null</c> when they are not limited.
    /// Throws 403 for a user with no ERAS role.
    /// </summary>
    Task<IReadOnlyCollection<int>?> GetVisibleStudentIdsAsync(CancellationToken CancellationToken = default);

    /// <summary>Throws 403 when the current user may not reach the student; callers check that the student exists first.</summary>
    Task EnsureCanAccessStudentAsync(int StudentId, CancellationToken CancellationToken = default);
}
