using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IInterventionActivityRepository
{
    /// <summary>Adds the events to the current unit of work.</summary>
    Task AddRangeAsync(IReadOnlyCollection<InterventionActivity> Activities, CancellationToken CancellationToken = default);

    /// <summary>The events of one intervention, in no guaranteed order.</summary>
    Task<IReadOnlyList<InterventionActivity>> GetByInterventionIdAsync(int InterventionId, CancellationToken CancellationToken = default);
}
