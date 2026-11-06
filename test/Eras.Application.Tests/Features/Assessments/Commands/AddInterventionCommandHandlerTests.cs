
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

internal sealed class TestIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Individual;
}

public class AddInterventionCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IMapper<IndividualInterventionDto, IndividualIntervention>> _mockIndividualMapper;
    private readonly Mock<IMapper<GroupInterventionDto, GroupIntervention>> _mockGroupMapper;
    private readonly Mock<IAttachmentService> _mockAttachmentService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserIdentityProvider> _mockUserIdentityProvider;
    private readonly Mock<ICurrentUserService> _mockCurrentUser;
    private readonly Mock<IInterventionActivityRepository> _mockActivityRepository;
    private readonly AddInterventionCommandHandler _handler;

    public AddInterventionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockIndividualMapper = new Mock<IMapper<IndividualInterventionDto, IndividualIntervention>>();
        _mockGroupMapper = new Mock<IMapper<GroupInterventionDto, GroupIntervention>>();
        _mockAttachmentService = new Mock<IAttachmentService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserIdentityProvider = new Mock<IUserIdentityProvider>();
        _mockUserIdentityProvider.Setup(X => X.UserId).Returns("user-1");
        _mockCurrentUser = new Mock<ICurrentUserService>();
        _mockCurrentUser.Setup(X => X.Sub).Returns("user-1");
        _mockCurrentUser.Setup(X => X.Name).Returns("Ana Pérez");
        _mockActivityRepository = new Mock<IInterventionActivityRepository>();

        // Participate in the "transaction" by simply invoking the passed work, mirroring
        // IUnitOfWork's real join-ambient-transaction behavior for these unit tests.
        _mockUnitOfWork
            .Setup(X => X.ExecuteInTransactionAsync(It.IsAny<Func<Task<Intervention>>>()))
            .Returns<Func<Task<Intervention>>>(Work => Work());

        _handler = new AddInterventionCommandHandler(
            _mockRepository.Object,
            _mockIndividualMapper.Object,
            _mockGroupMapper.Object,
            _mockAttachmentService.Object,
            _mockUnitOfWork.Object,
            _mockUserIdentityProvider.Object,
            _mockCurrentUser.Object,
            _mockActivityRepository.Object);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-1")]
    public async Task HandleAsync_NewIntervention_RecordsCreatedEventAsync()
    {
        // Arrange: an ERAS Professional named "Ana Pérez" creates an intervention
        var dto = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1],
            Activity = "Interview"
        };
        var mapped = new IndividualIntervention { DateUtc = dto.DateUtc, Status = dto.Status, StudentIds = dto.StudentIds };
        var persisted = new IndividualIntervention { Id = 10, DateUtc = dto.DateUtc, Status = dto.Status, StudentIds = dto.StudentIds };
        var assessment = new Assessment { Id = 1, CreatedBy = "", Service = "", Status = AssessmentStatus.Remitted, StudentIds = [1] };

        _mockRepository.Setup(r => r.GetByIdWithInterventionsAsync(1)).ReturnsAsync(assessment);
        _mockIndividualMapper.Setup(m => m.Map(dto)).Returns(mapped);
        _mockRepository.Setup(r => r.AddInterventionAsync(1, mapped)).ReturnsAsync(persisted);

        List<InterventionActivity> recorded = new();
        _mockActivityRepository
            .Setup(r => r.AddRangeAsync(It.IsAny<IReadOnlyCollection<InterventionActivity>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<InterventionActivity>, CancellationToken>((Items, _) => recorded.AddRange(Items))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(new AddInterventionCommand(1, dto), CancellationToken.None);

        // Assert: exactly one "Created" event with the user name and a UTC time
        InterventionActivity activity = Assert.Single(recorded);
        Assert.Equal(InterventionActivityType.Created, activity.Type);
        Assert.Equal(10, activity.InterventionId);
        Assert.Equal("Ana Pérez", activity.ActorName);
        Assert.Equal("user-1", activity.ActorSub);
        Assert.Equal(DateTimeKind.Utc, activity.OccurredAtUtc.Kind);
    }

    [Fact]
    public async Task HandleAddIndividualIntervention()
    {
        // Arrange
        var dto = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1],
            Activity = "Interview"
        };

        var command = new AddInterventionCommand(1, dto);

        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2]
        };

        var mappedIntervention = new IndividualIntervention
        {
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        var persisted = new IndividualIntervention
        {
            Id = 10,
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        _mockRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(assessment);

        _mockIndividualMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedIntervention);

        _mockRepository
            .Setup(r => r.AddInterventionAsync(1, mappedIntervention))
            .ReturnsAsync(persisted);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<IndividualInterventionDto>(result);

        Assert.Equal(10, result.Id);
        Assert.Equal(dto.Activity, result.Activity);
        Assert.Equal(dto.Status, result.Status);

        _mockRepository.Verify(r => r.GetByIdWithInterventionsAsync(1), Times.Once);
        _mockIndividualMapper.Verify(m => m.Map(dto), Times.Once);
        _mockRepository.Verify(r => r.AddInterventionAsync(1, mappedIntervention), Times.Once);

        _mockGroupMapper.Verify(m => m.Map(It.IsAny<GroupInterventionDto>()), Times.Never);
    }

    [Fact]
    public async Task HandleAddGroupIntervention()
    {
        // Arrange
        var dto = new GroupInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1, 2],
            Activity = "Interview"
        };

        var command = new AddInterventionCommand(1, dto);

        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2]
        };

        var mappedIntervention = new GroupIntervention
        {
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        var persisted = new GroupIntervention
        {
            Id = 11,
            DateUtc = dto.DateUtc,
            Status = dto.Status,
            StudentIds = dto.StudentIds,
            Activity = dto.Activity
        };

        _mockRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(assessment);

        _mockGroupMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedIntervention);

        _mockRepository
            .Setup(r => r.AddInterventionAsync(1, mappedIntervention))
            .ReturnsAsync(persisted);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GroupInterventionDto>(result);

        Assert.Equal(11, result.Id);
        Assert.Equal(dto.Activity, result.Activity);
        Assert.Equal(dto.Status, result.Status);

        _mockRepository.Verify(r => r.GetByIdWithInterventionsAsync(1), Times.Once);
        _mockGroupMapper.Verify(m => m.Map(dto), Times.Once);
        _mockRepository.Verify(r => r.AddInterventionAsync(1, mappedIntervention), Times.Once);

        _mockIndividualMapper.Verify(m => m.Map(It.IsAny<IndividualInterventionDto>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_NotClaimAttachments_When_NoDraftSessionIdProvidedAsync()
    {
        // Arrange
        var dto = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1],
            Activity = "Interview"
        };
        var command = new AddInterventionCommand(1, dto);
        var assessment = new Assessment { Id = 1, CreatedBy = "", Service = "", Status = AssessmentStatus.Remitted, StudentIds = [1] };
        var mappedIntervention = new IndividualIntervention { DateUtc = dto.DateUtc, Status = dto.Status, StudentIds = dto.StudentIds, Activity = dto.Activity };
        var persisted = new IndividualIntervention { Id = 10, DateUtc = dto.DateUtc, Status = dto.Status, StudentIds = dto.StudentIds, Activity = dto.Activity };

        _mockRepository.Setup(r => r.GetByIdWithInterventionsAsync(1)).ReturnsAsync(assessment);
        _mockIndividualMapper.Setup(m => m.Map(dto)).Returns(mappedIntervention);
        _mockRepository.Setup(r => r.AddInterventionAsync(1, mappedIntervention)).ReturnsAsync(persisted);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockAttachmentService.Verify(X => X.ClaimDraftAttachmentsAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ClaimDraftAttachments_ForThePersistedIntervention_When_DraftSessionIdProvidedAsync()
    {
        // Arrange
        var dto = new IndividualInterventionDto
        {
            DateUtc = DateTime.UtcNow,
            Status = InterventionStatus.Remitted,
            StudentIds = [1],
            Activity = "Interview"
        };
        var command = new AddInterventionCommand(1, dto, DraftSessionId: 5);
        var assessment = new Assessment { Id = 1, CreatedBy = "", Service = "", Status = AssessmentStatus.Remitted, StudentIds = [1] };
        var mappedIntervention = new IndividualIntervention { DateUtc = dto.DateUtc, Status = dto.Status, StudentIds = dto.StudentIds, Activity = dto.Activity };
        var persisted = new IndividualIntervention { Id = 42, DateUtc = dto.DateUtc, Status = dto.Status, StudentIds = dto.StudentIds, Activity = dto.Activity };

        _mockRepository.Setup(r => r.GetByIdWithInterventionsAsync(1)).ReturnsAsync(assessment);
        _mockIndividualMapper.Setup(m => m.Map(dto)).Returns(mappedIntervention);
        _mockRepository.Setup(r => r.AddInterventionAsync(1, mappedIntervention)).ReturnsAsync(persisted);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(42, result.Id);
        _mockUnitOfWork.Verify(X => X.ExecuteInTransactionAsync(It.IsAny<Func<Task<Intervention>>>()), Times.Once);
        _mockAttachmentService.Verify(X => X.ClaimDraftAttachmentsAsync(
            5, InterventionConstants.AttachmentEntityType, 42, "user-1", It.IsAny<CancellationToken>()), Times.Once);
    }
}
