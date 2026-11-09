using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class InterventionActivityToDtoMapper : IMapper<InterventionActivity, InterventionActivityDto>
{
    public InterventionActivityDto Map(InterventionActivity source) => new(
        source.Id,
        source.Type,
        Describe(source),
        DateTime.SpecifyKind(source.OccurredAtUtc, DateTimeKind.Utc),
        source.ActorName);

    private static string Describe(InterventionActivity Activity) => Activity.Type switch
    {
        InterventionActivityType.Created => "Intervention created",
        InterventionActivityType.CommentAdded => "Comment added",
        InterventionActivityType.StatusChanged =>
            $"Status changed: {Label(Activity.PreviousStatus)} → {Label(Activity.NewStatus)}",
        InterventionActivityType.RiskLevelChanged =>
            $"Risk level changed: {Label(Activity.PreviousRiskLevel)} → {Label(Activity.NewRiskLevel)}",
        InterventionActivityType.EndRiskLevelChanged =>
            $"End risk level changed: {Label(Activity.PreviousRiskLevel)} → {Label(Activity.NewRiskLevel)}",
        _ => throw new NotSupportedException($"Activity type '{Activity.Type}' is not supported.")
    };

    private static string Label(InterventionStatus? Status) => Status switch
    {
        InterventionStatus.Remitted => "Remitted",
        InterventionStatus.InProgress => "In Progress",
        InterventionStatus.Finalized => "Finalized",
        _ => Status?.ToString() ?? string.Empty
    };

    /// <summary>The label of a risk level; a missing value is shown as "None".</summary>
    private static string Label(InterventionLevel? Level) => Level?.ToString() ?? "None";
}
