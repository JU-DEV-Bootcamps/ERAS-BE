using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Features.RemissionManagement;

/// <summary>Decides which events a save of an assessment produces; it only compares values.</summary>
public static class AssessmentActivityFactory
{
    public static AssessmentActivity Created(int AssessmentId, ActivityActor Actor, DateTime NowUtc) =>
        Build(AssessmentId, AssessmentActivityType.Created, Actor, NowUtc);

    /// <summary>
    /// A comment event when the comments go from empty to text or change to a different text (clearing records
    /// nothing), then a status event when the status differs.
    /// </summary>
    public static IReadOnlyList<AssessmentActivity> FromChange(
        int AssessmentId,
        AssessmentStatus PreviousStatus,
        string? PreviousComments,
        AssessmentStatus NewStatus,
        string? NewComments,
        ActivityActor Actor,
        DateTime NowUtc)
    {
        List<AssessmentActivity> activities = new();

        bool hasNewComment = !string.IsNullOrWhiteSpace(NewComments);
        bool commentChanged = !string.Equals(
            string.IsNullOrWhiteSpace(PreviousComments) ? string.Empty : PreviousComments,
            NewComments,
            StringComparison.Ordinal);
        if (hasNewComment && commentChanged)
            activities.Add(Build(AssessmentId, AssessmentActivityType.CommentAdded, Actor, NowUtc));

        if (PreviousStatus != NewStatus)
        {
            AssessmentActivity statusChanged = Build(AssessmentId, AssessmentActivityType.StatusChanged, Actor, NowUtc);
            statusChanged.PreviousStatus = PreviousStatus;
            statusChanged.NewStatus = NewStatus;
            activities.Add(statusChanged);
        }

        return activities;
    }

    private static AssessmentActivity Build(
        int AssessmentId, AssessmentActivityType Type, ActivityActor Actor, DateTime NowUtc) =>
        new()
        {
            AssessmentId = AssessmentId,
            Type = Type,
            OccurredAtUtc = DateTime.SpecifyKind(NowUtc, DateTimeKind.Utc),
            ActorSub = Actor.Sub,
            ActorName = Actor.Name
        };
}
