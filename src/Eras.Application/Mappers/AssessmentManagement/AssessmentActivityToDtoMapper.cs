using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class AssessmentActivityToDtoMapper : IMapper<AssessmentActivity, AssessmentActivityDto>
{
    public AssessmentActivityDto Map(AssessmentActivity source) => new(
        source.Id,
        source.Type,
        Describe(source),
        DateTime.SpecifyKind(source.OccurredAtUtc, DateTimeKind.Utc),
        source.ActorName);

    private static string Describe(AssessmentActivity Activity) => Activity.Type switch
    {
        AssessmentActivityType.Created => "Assessment created",
        AssessmentActivityType.CommentAdded => "Comment added",
        AssessmentActivityType.StatusChanged =>
            $"Status changed: {Label(Activity.PreviousStatus)} → {Label(Activity.NewStatus)}",
        _ => throw new NotSupportedException($"Activity type '{Activity.Type}' is not supported.")
    };

    private static string Label(AssessmentStatus? Status) => Status switch
    {
        AssessmentStatus.Remitted => "Remitted",
        AssessmentStatus.InProgress => "In Progress",
        AssessmentStatus.Finalized => "Finalized",
        _ => Status?.ToString() ?? string.Empty
    };
}
