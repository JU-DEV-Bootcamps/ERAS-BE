using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IAssessmentActivityRepository
{
    /// <summary>Adds the events to the current unit of work.</summary>
    Task AddRangeAsync(IReadOnlyCollection<AssessmentActivity> Activities, CancellationToken CancellationToken = default);

    /// <summary>The events of one assessment, in no guaranteed order.</summary>
    Task<IReadOnlyList<AssessmentActivity>> GetByAssessmentIdAsync(int AssessmentId, CancellationToken CancellationToken = default);
}
