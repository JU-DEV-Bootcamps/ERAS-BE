using Eras.Application.Contracts.Infrastructure;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Features.RemissionManagement;

/// <summary>Who performed an action: the Keycloak sub and the display name at that moment.</summary>
public sealed record ActivityActor(string Sub, string Name)
{
    private const int MaxLength = 200;

    /// <summary>Same rule as the student intervention entries: <c>Name ?? Email ?? Sub</c>, truncated to 200.</summary>
    public static ActivityActor From(ICurrentUserService CurrentUser)
    {
        string sub = CurrentUser.Sub ?? "System";
        string name = CurrentUser.Name ?? CurrentUser.Email ?? sub;
        return new ActivityActor(Truncate(sub), Truncate(name));
    }

    private static string Truncate(string Value) => Value.Length <= MaxLength ? Value : Value[..MaxLength];
}

/// <summary>Decides which events a save of an intervention produces; it only compares values.</summary>
public static class InterventionActivityFactory
{
    public static InterventionActivity Created(int InterventionId, ActivityActor Actor, DateTime NowUtc) =>
        Build(InterventionId, InterventionActivityType.Created, Actor, NowUtc);

    /// <summary>
    /// A comment event when the comments go from empty to text or change to a different text (clearing records
    /// nothing), then a status event when the status differs.
    /// </summary>
    public static IReadOnlyList<InterventionActivity> FromChange(
        int InterventionId,
        InterventionStatus PreviousStatus,
        string? PreviousComments,
        InterventionStatus NewStatus,
        string? NewComments,
        ActivityActor Actor,
        DateTime NowUtc)
    {
        List<InterventionActivity> activities = new();

        bool hasNewComment = !string.IsNullOrWhiteSpace(NewComments);
        bool commentChanged = !string.Equals(
            string.IsNullOrWhiteSpace(PreviousComments) ? string.Empty : PreviousComments,
            NewComments,
            StringComparison.Ordinal);
        if (hasNewComment && commentChanged)
            activities.Add(Build(InterventionId, InterventionActivityType.CommentAdded, Actor, NowUtc));

        if (PreviousStatus != NewStatus)
        {
            InterventionActivity statusChanged = Build(InterventionId, InterventionActivityType.StatusChanged, Actor, NowUtc);
            statusChanged.PreviousStatus = PreviousStatus;
            statusChanged.NewStatus = NewStatus;
            activities.Add(statusChanged);
        }

        return activities;
    }

    private static InterventionActivity Build(
        int InterventionId, InterventionActivityType Type, ActivityActor Actor, DateTime NowUtc) =>
        new()
        {
            InterventionId = InterventionId,
            Type = Type,
            OccurredAtUtc = DateTime.SpecifyKind(NowUtc, DateTimeKind.Utc),
            ActorSub = Actor.Sub,
            ActorName = Actor.Name
        };
}
