using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public sealed class GetAssessmentActivityQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _assessmentRepository = new();
    private readonly Mock<IAssessmentActivityRepository> _activityRepository = new();
    private readonly GetAssessmentActivityQueryHandler _handler;

    public GetAssessmentActivityQueryHandlerTests()
    {
        _handler = new GetAssessmentActivityQueryHandler(
            _assessmentRepository.Object,
            _activityRepository.Object,
            new AssessmentActivityToDtoMapper());
    }

    private static Assessment MakeAssessment(int Id) => new()
    {
        Id = Id,
        CreatedBy = "auto",
        Service = "Any",
        Status = AssessmentStatus.Remitted,
        StudentIds = [1]
    };

    private static AssessmentActivity MakeActivity(
        int Id, int AssessmentId, AssessmentActivityType Type, DateTime At, string User = "Ana Pérez") => new()
        {
            Id = Id,
            AssessmentId = AssessmentId,
            Type = Type,
            OccurredAtUtc = At,
            ActorSub = "sub",
            ActorName = User
        };

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-7")]
    public async Task HandleAsync_UnorderedEvents_ReturnsOldestFirstAsync()
    {
        // Arrange (GIVEN) three events stored out of order
        DateTime first = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);
        DateTime second = new(2026, 11, 3, 10, 15, 0, DateTimeKind.Utc);
        DateTime third = new(2026, 11, 4, 15, 30, 0, DateTimeKind.Utc);
        _assessmentRepository.Setup(R => R.GetByIdAsync(1)).ReturnsAsync(MakeAssessment(1));
        AssessmentActivity status = MakeActivity(2, 1, AssessmentActivityType.StatusChanged, second, "Luis Gómez");
        status.PreviousStatus = AssessmentStatus.Remitted;
        status.NewStatus = AssessmentStatus.InProgress;
        _activityRepository
            .Setup(R => R.GetByAssessmentIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentActivity>
            {
                MakeActivity(3, 1, AssessmentActivityType.CommentAdded, third),
                MakeActivity(1, 1, AssessmentActivityType.Created, first),
                status
            });

        // Act (WHEN) the history is requested
        IReadOnlyCollection<AssessmentActivityDto> result =
            await _handler.Handle(new GetAssessmentActivityQuery(1), CancellationToken.None);

        // Assert (THEN) oldest first, each with description, time and user name
        AssessmentActivityDto[] events = result.ToArray();
        Assert.Equal([first, second, third], events.Select(E => E.OccurredAtUtc));
        Assert.Equal("Assessment created", events[0].Description);
        Assert.Equal("Status changed: Remitted → In Progress", events[1].Description);
        Assert.Equal("Luis Gómez", events[1].UserName);
        Assert.Equal("Comment added", events[2].Description);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-11")]
    public async Task HandleAsync_OtherAssessmentHasEvents_ReturnsOnlyItsOwnAsync()
    {
        // Arrange (GIVEN) assessment 4 with two events and assessment 5 with one
        DateTime at = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);
        _assessmentRepository.Setup(R => R.GetByIdAsync(4)).ReturnsAsync(MakeAssessment(4));
        _assessmentRepository.Setup(R => R.GetByIdAsync(5)).ReturnsAsync(MakeAssessment(5));
        _activityRepository
            .Setup(R => R.GetByAssessmentIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentActivity>
            {
                MakeActivity(1, 4, AssessmentActivityType.Created, at),
                MakeActivity(2, 4, AssessmentActivityType.CommentAdded, at.AddDays(1))
            });
        _activityRepository
            .Setup(R => R.GetByAssessmentIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentActivity> { MakeActivity(3, 5, AssessmentActivityType.Created, at) });

        // Act (WHEN) the history of 5 is requested, then the one of 4
        IReadOnlyCollection<AssessmentActivityDto> forB =
            await _handler.Handle(new GetAssessmentActivityQuery(5), CancellationToken.None);
        IReadOnlyCollection<AssessmentActivityDto> forA =
            await _handler.Handle(new GetAssessmentActivityQuery(4), CancellationToken.None);

        // Assert (THEN) 5 only has its own event and 4 still has its two
        Assert.Equal(3, Assert.Single(forB).Id);
        Assert.Equal(2, forA.Count);
    }

    [Fact]
    public async Task HandleAsync_UnknownAssessment_ThrowsNotFoundAsync()
    {
        // Arrange (GIVEN) no assessment with id 999999
        _assessmentRepository.Setup(R => R.GetByIdAsync(999999)).ReturnsAsync((Assessment?)null);

        // Act (WHEN) its history is requested
        Func<Task> act = () => _handler.Handle(new GetAssessmentActivityQuery(999999), CancellationToken.None);

        // Assert (THEN) not found (404) and no event data read
        await Assert.ThrowsAsync<NotFoundException>(act);
        _activityRepository.Verify(
            R => R.GetByAssessmentIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-13")]
    public async Task HandleAsync_NoEvents_ReturnsEmptyListAsync()
    {
        // Arrange (GIVEN) an assessment created before this release, with no events
        _assessmentRepository.Setup(R => R.GetByIdAsync(1)).ReturnsAsync(MakeAssessment(1));
        _activityRepository
            .Setup(R => R.GetByAssessmentIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentActivity>());

        // Act (WHEN) its history is requested
        IReadOnlyCollection<AssessmentActivityDto> result =
            await _handler.Handle(new GetAssessmentActivityQuery(1), CancellationToken.None);

        // Assert (THEN) an empty list
        Assert.Empty(result);
    }
}
