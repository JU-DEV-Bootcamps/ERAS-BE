

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;
using Eras.Error.Bussiness;

using FluentValidation;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public sealed class ReplaceInterventionCommandHandlerTests
{
    private readonly Mock<IAttachmentService> _attachmentService;
    private readonly Mock<IAttachmentRepository> _attachmentRepository;
    private readonly Mock<IAssessmentRepository> _assessmentRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IUserIdentityProvider> _userIdentityProvider;
    private readonly Mock<ICurrentUserService> _currentUser;
    private readonly Mock<IInterventionActivityRepository> _activityRepository;
    private readonly List<InterventionActivity> _recordedActivities = new();
    private readonly Mock<ILogger<ReplaceInterventionCommandHandler>> _logger;
    private readonly Mock<IMapper<UpdateInterventionDto, Intervention>> _mapper;
    private readonly Mock<IValidator<StatusTransitionRequest<InterventionStatus>>> _statusValidator;

    private readonly ReplaceInterventionCommandHandler _handler;

    public ReplaceInterventionCommandHandlerTests()
    {
        _attachmentService = new Mock<IAttachmentService>();
        _attachmentRepository = new Mock<IAttachmentRepository>();
        _assessmentRepository = new Mock<IAssessmentRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _userIdentityProvider = new Mock<IUserIdentityProvider>();
        _currentUser = new Mock<ICurrentUserService>();
        _currentUser.Setup(U => U.Sub).Returns("user-1");
        _currentUser.Setup(U => U.Name).Returns("Luis Gómez");
        _activityRepository = new Mock<IInterventionActivityRepository>();
        _activityRepository
            .Setup(R => R.GetByInterventionIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterventionActivity>());
        _activityRepository
            .Setup(R => R.AddRangeAsync(It.IsAny<IReadOnlyCollection<InterventionActivity>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<InterventionActivity>, CancellationToken>((Items, _) => _recordedActivities.AddRange(Items))
            .Returns(Task.CompletedTask);
        _logger = new Mock<ILogger<ReplaceInterventionCommandHandler>>();
        _mapper = new Mock<IMapper<UpdateInterventionDto, Intervention>>();
        _statusValidator = new Mock<IValidator<StatusTransitionRequest<InterventionStatus>>>();

        _statusValidator
            .Setup(V => V.ValidateAsync(It.IsAny<StatusTransitionRequest<InterventionStatus>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new ReplaceInterventionCommandHandler(
            _assessmentRepository.Object,
            _attachmentRepository.Object,
            _attachmentService.Object,
            _unitOfWork.Object,
            _userIdentityProvider.Object,
            _currentUser.Object,
            _activityRepository.Object,
            _logger.Object,
            _mapper.Object,
            _statusValidator.Object
            );
    }

    private static UpdateInterventionDto MakeDto(InterventionStatus Status = InterventionStatus.Remitted) => new(
        DateUtc: DateTime.UtcNow,
        Activity: "activity",
        Area: "area",
        NumberOfParticipants: 1,
        Professional: null,
        Comments: null,
        StudentIds: Array.Empty<int>(),
        Attendance: new Dictionary<int, bool>(),
        Mode: InterventionMode.Remote,
        Kind: InterventionKind.Individual,
        Status: Status,
        Remarks: null,
        RiskLevel: null,
        RiskLevelName: InterventionLevel.Medium,
        EndRiskLevelName: null,
        Id: null);

    private static Intervention MakeIntervention(int Id, InterventionStatus Status = InterventionStatus.Remitted) => new()
    {
        Id = Id,
        DateUtc = DateTime.UtcNow,
        StudentIds = Array.Empty<int>(),
        Status = Status
    };

    private static Assessment MakeAssessment(params Intervention[] Interventions) => new()
    {
        Interventions = Interventions.ToList(),
        CreatedBy = "auto",
        Service = "Any",
        Status = AssessmentStatus.Remitted,
        StudentIds = [1]
    };

    private static Attachment MakeAttachment(int Id, int EntityId, string StorageKey) => new()
    {
        Id = Id,
        EntityType = InterventionConstants.AttachmentEntityType,
        EntityId = EntityId,
        StorageKey = StorageKey,
        ContentHash = "hash",
        CreatedBy = "tester"
    };

    private void SetupTransaction(Intervention Returns)
    {
        _unitOfWork
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<Intervention>>>()))
            .Returns<Func<Task<Intervention>>>(F => F());
        _assessmentRepository
            .Setup(R => R.AddInterventionAsync(It.IsAny<int>(), It.IsAny<Intervention>()))
            .ReturnsAsync(Returns);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-20")]
    public async Task HandleAsync_TypeChanged_KeepsTheEventsAsync()
    {
        // Arrange: an individual intervention (id 1) with two events
        DateTime created = new(2026, 11, 2, 9, 0, 0, DateTimeKind.Utc);
        DateTime commented = new(2026, 11, 4, 15, 30, 0, DateTimeKind.Utc);
        _activityRepository
            .Setup(R => R.GetByInterventionIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterventionActivity>
            {
                new() { Id = 7, InterventionId = 1, Type = InterventionActivityType.Created, OccurredAtUtc = created, ActorSub = "ana-sub", ActorName = "Ana Pérez" },
                new() { Id = 8, InterventionId = 1, Type = InterventionActivityType.CommentAdded, OccurredAtUtc = commented, ActorSub = "luis-sub", ActorName = "Luis Gómez" }
            });
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(MakeIntervention(1)));
        SetupTransaction(MakeIntervention(5));

        // Act: the user changes the type to group (the intervention is recreated with id 5)
        await _handler.Handle(
            new ReplaceInterventionCommand(1, 1, MakeDto() with { Kind = InterventionKind.Group }, null, null),
            CancellationToken.None);

        // Assert: the two events are kept on the new intervention, same order, users and times, and no new "Created"
        Assert.Equal(2, _recordedActivities.Count);
        Assert.All(_recordedActivities, A => Assert.Equal(5, A.InterventionId));
        Assert.Equal(InterventionActivityType.Created, _recordedActivities[0].Type);
        Assert.Equal(created, _recordedActivities[0].OccurredAtUtc);
        Assert.Equal("Ana Pérez", _recordedActivities[0].ActorName);
        Assert.Equal(InterventionActivityType.CommentAdded, _recordedActivities[1].Type);
        Assert.Equal(commented, _recordedActivities[1].OccurredAtUtc);
        Assert.Equal("Luis Gómez", _recordedActivities[1].ActorName);
        Assert.All(_recordedActivities, A => Assert.Equal(0, A.Id));
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenAssessmentDoesNotExistAsync()
    {
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(It.IsAny<int>()))
            .ReturnsAsync((Assessment?)null);

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), null, null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenInterventionDoesNotBelongToAssessmentAsync()
    {
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(MakeIntervention(99)));

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), null, null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowBussinessException_WhenAttachmentIdDoesNotBelongToInterventionAsync()
    {
        var existing = MakeIntervention(1);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _attachmentRepository
            .Setup(R => R.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new[] { MakeAttachment(10, 1, "interventions/1/file.pdf") });

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), new[] { 999 }, null);

        await Assert.ThrowsAsync<BussinessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ReplaceIntervention_WithNoAttachmentChangesAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        _assessmentRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _mapper
            .Setup(m => m.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);
        SetupTransaction(persisted);

        _attachmentRepository
            .Setup(r => r.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(Array.Empty<Attachment>());

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), null, null);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(persisted.Id, result.Id);
        _assessmentRepository.Verify(
            r => r.DeleteInterventionAsync(1, 1), Times.Once);
        _assessmentRepository.Verify(
            r => r.AddInterventionAsync(1, It.IsAny<Intervention>()), Times.Once);
        _attachmentRepository.Verify(
            r => r.DeleteByIdsAsync(It.IsAny<int[]>()),
            Times.Never);

        _attachmentService.Verify(
            s => s.ClaimDraftAttachmentsAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_DeleteAttachmentRowsInsideTransaction_AndPhysicallyDeleteAfterCommitAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        var attachment = MakeAttachment(10, 1, "interventions/1/file.pdf");
        _assessmentRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _attachmentRepository
            .Setup(r => r.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new[] { attachment });
        _mapper
            .Setup(m => m.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);
        SetupTransaction(persisted);

        _attachmentRepository
            .Setup(r => r.DeleteByIdsAsync(new[] { 10 }))
            .ReturnsAsync(1);
        _attachmentService
            .Setup(s => s.DeleteByStorageKeyAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), new[] { 10 }, null);

        await _handler.Handle(command, CancellationToken.None);

        _attachmentRepository.Verify(
            r => r.DeleteByIdsAsync(new[] { 10 }), Times.Once);

        _attachmentService.Verify(
            s => s.DeleteByStorageKeyAsync(
                "interventions/1/file.pdf",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ClaimDraftAttachments_WhenDraftSessionIdProvidedAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);

        _assessmentRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));

        _mapper
            .Setup(m => m.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);

        SetupTransaction(persisted);

        _attachmentRepository
            .Setup(r => r.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(Array.Empty<Attachment>());

        _attachmentService
            .Setup(s => s.ClaimDraftAttachmentsAsync(
                7,
                InterventionConstants.AttachmentEntityType,
                1,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userIdentityProvider
            .Setup(p => p.UserId)
            .Returns("user-1");

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), null, 7);

        await _handler.Handle(command, CancellationToken.None);

        _attachmentService.Verify(
            s => s.ClaimDraftAttachmentsAsync(
                7,
                InterventionConstants.AttachmentEntityType,
                1,
                "user-1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_LogWarning_AndNotThrow_WhenPhysicalFileDeletionFailsAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        var attachment = MakeAttachment(10, 1, "interventions/1/file.pdf");

        _assessmentRepository
            .Setup(r => r.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));

        _attachmentRepository
            .Setup(r => r.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new[] { attachment });

        _mapper
            .Setup(m => m.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);

        SetupTransaction(persisted);

        _attachmentRepository
            .Setup(r => r.DeleteByIdsAsync(It.IsAny<int[]>()))
            .ReturnsAsync(1);

        _attachmentService
            .Setup(s => s.DeleteByStorageKeyAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("disk error"));

        var command = new ReplaceInterventionCommand(1, 1, MakeDto(), new[] { 10 }, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);

        _logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<IOException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_AddProfessionalToAssessment_WhenProfessionalSubProvidedAsync()
    {
        var old = MakeIntervention(1);
        var created = MakeIntervention(5);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(old));
        _mapper.Setup(M => M.Map(It.IsAny<UpdateInterventionDto>())).Returns(created);
        SetupTransaction(created);
        _attachmentRepository
            .Setup(R => R.GetByEntityAsync(InterventionConstants.AttachmentEntityType, It.IsAny<int>()))
            .ReturnsAsync(Array.Empty<Attachment>());
        UpdateInterventionDto dto = MakeDto() with { Professional = "Luis Gómez", ProfessionalSub = "luis-sub" };

        await _handler.Handle(new ReplaceInterventionCommand(1, 1, dto, null, null), CancellationToken.None);

        _assessmentRepository.Verify(R => R.AddAssignedProfessionalAsync(1, "luis-sub"), Times.Once);
    }
}
