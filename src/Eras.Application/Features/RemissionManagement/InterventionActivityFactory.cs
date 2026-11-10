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
    /// nothing), then a status event, a risk level event and an end risk level event when each value differs.
    /// The risk level values are optional: a caller that does not pass them records no risk level event.
    /// </summary>
    public static IReadOnlyList<InterventionActivity> FromChange(
        int InterventionId,
        InterventionStatus PreviousStatus,
        string? PreviousComments,
        InterventionStatus NewStatus,
        string? NewComments,
        ActivityActor Actor,
        DateTime NowUtc,
        InterventionLevel? PreviousRiskLevel = null,
        InterventionLevel? NewRiskLevel = null,
        InterventionLevel? PreviousEndRiskLevel = null,
        InterventionLevel? NewEndRiskLevel = null)
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

        AddRiskLevelChange(
            activities, InterventionActivityType.RiskLevelChanged, InterventionId, PreviousRiskLevel, NewRiskLevel, Actor, NowUtc);
        AddRiskLevelChange(
            activities, InterventionActivityType.EndRiskLevelChanged, InterventionId, PreviousEndRiskLevel, NewEndRiskLevel, Actor, NowUtc);

        return activities;
    }

    private static void AddRiskLevelChange(
        List<InterventionActivity> Activities,
        InterventionActivityType Type,
        int InterventionId,
        InterventionLevel? Previous,
        InterventionLevel? New,
        ActivityActor Actor,
        DateTime NowUtc)
    {
        if (Previous == New)
            return;

        InterventionActivity change = Build(InterventionId, Type, Actor, NowUtc);
        change.PreviousRiskLevel = Previous;
        change.NewRiskLevel = New;
        Activities.Add(change);
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
