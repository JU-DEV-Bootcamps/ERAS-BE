using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public sealed class GetInterventionActivityQueryHandlerTests
{

    private readonly Mock<IAssessmentRepository> _assessmentRepository = new();
    private readonly Mock<IInterventionActivityRepository> _activityRepository = new();
    private readonly GetInterventionActivityQueryHandler _handler;

    public GetInterventionActivityQueryHandlerTests()
    {
        _handler = new GetInterventionActivityQueryHandler(
            _assessmentRepository.Object,
            _activityRepository.Object,
            new InterventionActivityToDtoMapper());
    }

    private static Intervention MakeIntervention(int Id) => new()
    {
        Id = Id,
        DateUtc = DateTime.UtcNow,
        StudentIds = Array.Empty<int>()
    };

    private static Assessment MakeAssessment(params Intervention[] Interventions) => new()
    {
        Id = 1,
        Interventions = Interventions.ToList(),
        CreatedBy = "auto",
        Service = "Any",
        Status = AssessmentStatus.Remitted,
        StudentIds = [1]
    };

    private static InterventionActivity MakeActivity(
        int Id, int InterventionId, InterventionActivityType Type, DateTime At, string User = "Ana Pérez") => new()
        {
            Id = Id,
            InterventionId = InterventionId,
            Type = Type,
            OccurredAtUtc = At,
            ActorSub = "sub",
            ActorName = User
        };

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-7")]
    public async Task HandleAsync_UnorderedEvents_ReturnsOldestFirstAsync()
    {
        // Arrange: three events stored out of order
        DateTime first = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);
        DateTime second = new(2026, 11, 3, 10, 15, 0, DateTimeKind.Utc);
        DateTime third = new(2026, 11, 4, 15, 30, 0, DateTimeKind.Utc);
        _assessmentRepository.Setup(R => R.GetByIdWithInterventionsAsync(1)).ReturnsAsync(MakeAssessment(MakeIntervention(5)));
        InterventionActivity status = MakeActivity(2, 5, InterventionActivityType.StatusChanged, second, "Luis Gómez");
        status.PreviousStatus = InterventionStatus.Remitted;
        status.NewStatus = InterventionStatus.InProgress;
        _activityRepository
            .Setup(R => R.GetByInterventionIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterventionActivity>
            {
                MakeActivity(3, 5, InterventionActivityType.CommentAdded, third),
                MakeActivity(1, 5, InterventionActivityType.Created, first),
                status
            });

        // Act
        IReadOnlyCollection<InterventionActivityDto> result =
            await _handler.Handle(new GetInterventionActivityQuery(1, 5), CancellationToken.None);

        // Assert: oldest first, each with description, time and user name
        InterventionActivityDto[] events = result.ToArray();
        Assert.Equal([first, second, third], events.Select(E => E.OccurredAtUtc));
        Assert.Equal("Intervention created", events[0].Description);
        Assert.Equal("Status changed: Remitted → In Progress", events[1].Description);
        Assert.Equal("Luis Gómez", events[1].UserName);
        Assert.Equal("Comment added", events[2].Description);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-11")]
    public async Task HandleAsync_OtherInterventionHasEvents_ReturnsOnlyItsOwnAsync()
    {
        // Arrange: intervention A (id 4) with two events and intervention B (id 5) with one
        DateTime at = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(MakeIntervention(4), MakeIntervention(5)));
        _activityRepository
            .Setup(R => R.GetByInterventionIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterventionActivity>
            {
                MakeActivity(1, 4, InterventionActivityType.Created, at),
                MakeActivity(2, 4, InterventionActivityType.CommentAdded, at.AddDays(1))
            });
        _activityRepository
            .Setup(R => R.GetByInterventionIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterventionActivity> { MakeActivity(3, 5, InterventionActivityType.Created, at) });

        // Act
        IReadOnlyCollection<InterventionActivityDto> forB =
            await _handler.Handle(new GetInterventionActivityQuery(1, 5), CancellationToken.None);
        IReadOnlyCollection<InterventionActivityDto> forA =
            await _handler.Handle(new GetInterventionActivityQuery(1, 4), CancellationToken.None);

        // Assert: B only has its own event and A still has its two
        Assert.Equal(3, Assert.Single(forB).Id);
        Assert.Equal(2, forA.Count);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-12")]
    public async Task HandleAsync_UnknownIntervention_ThrowsKeyNotFoundAsync()
    {
        // Arrange: an assessment without an intervention with id 999999
        _assessmentRepository.Setup(R => R.GetByIdWithInterventionsAsync(1)).ReturnsAsync(MakeAssessment(MakeIntervention(5)));

        // Act
        Func<Task> act = () => _handler.Handle(new GetInterventionActivityQuery(1, 999999), CancellationToken.None);

        // Assert: not found (404) and no event data read
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
        _activityRepository.Verify(
            R => R.GetByInterventionIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-13")]
    public async Task HandleAsync_NoEvents_ReturnsEmptyListAsync()
    {
        // Arrange: an intervention created before this release, with no events
        _assessmentRepository.Setup(R => R.GetByIdWithInterventionsAsync(1)).ReturnsAsync(MakeAssessment(MakeIntervention(5)));
        _activityRepository
            .Setup(R => R.GetByInterventionIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterventionActivity>());

        // Act
        IReadOnlyCollection<InterventionActivityDto> result =
            await _handler.Handle(new GetInterventionActivityQuery(1, 5), CancellationToken.None);

        // Assert: an empty list and the intervention is unchanged
        Assert.Empty(result);
        _assessmentRepository.Verify(R => R.UpdateInterventionAsync(It.IsAny<int>(), It.IsAny<Intervention>()), Times.Never);
    }
}
