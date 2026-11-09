using Eras.Application.Features.RemissionManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Features.Assessments;

public sealed class AssessmentActivityFactoryTests
{
    private static readonly ActivityActor Actor = new("sub-1", "Luis Gómez");
    private static readonly DateTime Now = new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);

    private static IReadOnlyList<AssessmentActivity> Change(
        AssessmentStatus PreviousStatus, string? PreviousComments, AssessmentStatus NewStatus, string? NewComments) =>
        AssessmentActivityFactory.FromChange(1, PreviousStatus, PreviousComments, NewStatus, NewComments, Actor, Now);

    [Fact]
    public void Created_BuildsCreatedEventWithActorAndUtcTime()
    {
        AssessmentActivity activity = AssessmentActivityFactory.Created(7, Actor, Now);

        Assert.Equal(AssessmentActivityType.Created, activity.Type);
        Assert.Equal(7, activity.AssessmentId);
        Assert.Equal("Luis Gómez", activity.ActorName);
        Assert.Equal("sub-1", activity.ActorSub);
        Assert.Equal(DateTimeKind.Utc, activity.OccurredAtUtc.Kind);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void FromChange_EmptyToText_RecordsCommentAdded(string? Previous)
    {
        IReadOnlyList<AssessmentActivity> result = Change(AssessmentStatus.Remitted, Previous, AssessmentStatus.Remitted, "Family contacted");

        Assert.Equal(AssessmentActivityType.CommentAdded, Assert.Single(result).Type);
    }

    [Fact]
    public void FromChange_TextToDifferentText_RecordsCommentAdded()
    {
        IReadOnlyList<AssessmentActivity> result = Change(AssessmentStatus.Remitted, "Family contacted", AssessmentStatus.Remitted, "Family met");

        Assert.Equal(AssessmentActivityType.CommentAdded, Assert.Single(result).Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FromChange_TextCleared_RecordsNothing(string? Cleared)
    {
        Assert.Empty(Change(AssessmentStatus.Remitted, "Family contacted", AssessmentStatus.Remitted, Cleared));
    }

    [Fact]
    public void FromChange_SameTextAndStatus_RecordsNothing()
    {
        Assert.Empty(Change(AssessmentStatus.Remitted, "Family contacted", AssessmentStatus.Remitted, "Family contacted"));
    }

    [Fact]
    public void FromChange_StatusChanged_RecordsPreviousAndNew()
    {
        AssessmentActivity activity = Assert.Single(
            Change(AssessmentStatus.Remitted, null, AssessmentStatus.InProgress, null));

        Assert.Equal(AssessmentActivityType.StatusChanged, activity.Type);
        Assert.Equal(AssessmentStatus.Remitted, activity.PreviousStatus);
        Assert.Equal(AssessmentStatus.InProgress, activity.NewStatus);
    }

    [Fact]
    public void FromChange_CommentAndStatus_RecordsCommentThenStatus()
    {
        IReadOnlyList<AssessmentActivity> result = Change(AssessmentStatus.Remitted, null, AssessmentStatus.InProgress, "Family contacted");

        Assert.Equal(
            [AssessmentActivityType.CommentAdded, AssessmentActivityType.StatusChanged],
            result.Select(A => A.Type));
    }
}
