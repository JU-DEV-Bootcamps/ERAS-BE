
namespace Eras.Domain.Entities.RemissionsManagement;

public sealed class InterventionPlan : BaseEntity
{
    /// <summary>
    /// Example: 3 interventions per week.
    /// </summary>
    public int? SessionsPerWeek { get; init; }

    /// <summary>
    /// Free-form scheduling notes until the scheduling rules are clearer.
    /// </summary>
    public string? ScheduleNotes { get; init; }
}