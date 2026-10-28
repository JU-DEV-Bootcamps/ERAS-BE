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
using FluentValidation.Results;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public sealed class UpdateInterventionCommandHandlerTests
{
    private readonly Mock<IAttachmentService> _attachmentService;
    private readonly Mock<IAttachmentRepository> _attachmentRepository;
    private readonly Mock<IAssessmentRepository> _assessmentRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IUserIdentityProvider> _userIdentityProvider;
    private readonly Mock<ILogger<UpdateInterventionCommandHandler>> _logger;
    private readonly Mock<IMapper<UpdateInterventionDto, Intervention>> _mapper;
    private readonly Mock<IValidator<StatusTransitionRequest<InterventionStatus>>> _statusValidator;

    private readonly UpdateInterventionCommandHandler _handler;

    public UpdateInterventionCommandHandlerTests()
    {
        _attachmentService = new Mock<IAttachmentService>();
        _attachmentRepository = new Mock<IAttachmentRepository>();
        _assessmentRepository = new Mock<IAssessmentRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _userIdentityProvider = new Mock<IUserIdentityProvider>();
        _logger = new Mock<ILogger<UpdateInterventionCommandHandler>>();
        _mapper = new Mock<IMapper<UpdateInterventionDto, Intervention>>();
        _statusValidator = new Mock<IValidator<StatusTransitionRequest<InterventionStatus>>>();

        _statusValidator
            .Setup(V => V.ValidateAsync(It.IsAny<StatusTransitionRequest<InterventionStatus>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new UpdateInterventionCommandHandler(
            _attachmentService.Object,
            _attachmentRepository.Object,
            _assessmentRepository.Object,
            _unitOfWork.Object,
            _userIdentityProvider.Object,
            _logger.Object,
            _mapper.Object,
            _statusValidator.Object
            );
    }

    private static UpdateInterventionDto MakeDto(
        InterventionStatus Status = InterventionStatus.Remitted,
        string? Professional = null,
        string? ProfessionalSub = null,
        string? Comments = null) => new(
        DateUtc: DateTime.UtcNow,
        Activity: "activity",
        Area: "area",
        NumberOfParticipants: 1,
        Professional: Professional,
        Comments: Comments,
        StudentIds: Array.Empty<int>(),
        Attendance: new Dictionary<int, bool>(),
        Mode: InterventionMode.Remote,
        Kind: InterventionKind.Individual,
        Status: Status,
        Remarks: null,
        RiskLevel: null,
        RiskLevelName: InterventionLevel.Medium,
        EndRiskLevelName: null,
        Id: null,
        ProfessionalSub: ProfessionalSub);

    private static Intervention MakeIntervention(int Id, InterventionStatus Status = InterventionStatus.Remitted) => new()
    {
        Id = Id,
        DateUtc = DateTime.UtcNow,
        StudentIds = Array.Empty<int>(),
        Status = Status
    };

    private static Assessment MakeAssessment(params Intervention[] Interventions) => MakeAssessmentWith([], Interventions);

    private static Assessment MakeAssessmentWith(string[] Professionals, params Intervention[] Interventions) => new()
    {
        AssignedProfessionals = Professionals,
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
            .Setup(R => R.UpdateInterventionAsync(It.IsAny<int>(), It.IsAny<Intervention>()))
            .ReturnsAsync(Returns);
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenAssessmentDoesNotExistAsync()
    {
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(It.IsAny<int>()))
            .ReturnsAsync((Assessment?)null);

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), null, null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_WhenInterventionDoesNotBelongToAssessmentAsync()
    {
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(MakeIntervention(99)));

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), null, null);

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

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), new[] { 999 }, null);

        await Assert.ThrowsAsync<BussinessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_UpdateIntervention_WithNoAttachmentChangesAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _mapper
            .Setup(M => M.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);
        SetupTransaction(persisted);
        _attachmentRepository
            .Setup(R => R.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(Array.Empty<Attachment>());

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), null, null);

        UpdateInterventionDto result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(persisted.Id, result.Id);
        _attachmentRepository.Verify(R => R.DeleteByIdsAsync(It.IsAny<int[]>()), Times.Never);
        _attachmentService.Verify(S => S.ClaimDraftAttachmentsAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_DeleteAttachmentRowsInsideTransaction_AndPhysicallyDeleteAfterCommitAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        var attachment = MakeAttachment(10, 1, "interventions/1/file.pdf");

        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _attachmentRepository
            .Setup(R => R.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new[] { attachment });
        _mapper
            .Setup(M => M.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);
        SetupTransaction(persisted);
        _attachmentRepository
            .Setup(R => R.DeleteByIdsAsync(new[] { 10 }))
            .ReturnsAsync(1);
        _attachmentService
            .Setup(S => S.DeleteByStorageKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), new[] { 10 }, null);

        await _handler.Handle(command, CancellationToken.None);

        _attachmentRepository.Verify(R => R.DeleteByIdsAsync(new[] { 10 }), Times.Once);
        _attachmentService.Verify(S => S.DeleteByStorageKeyAsync("interventions/1/file.pdf", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ClaimDraftAttachments_WhenDraftSessionIdProvidedAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _mapper
            .Setup(M => M.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);
        SetupTransaction(persisted);
        _attachmentService
            .Setup(S => S.ClaimDraftAttachmentsAsync(7, InterventionConstants.AttachmentEntityType, 1,
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _attachmentRepository
            .Setup(R => R.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(Array.Empty<Attachment>());
        _userIdentityProvider
            .Setup(P => P.UserId)
            .Returns("user-1");

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), null, 7);

        await _handler.Handle(command, CancellationToken.None);

        _attachmentService.Verify(S => S.ClaimDraftAttachmentsAsync(
            7, InterventionConstants.AttachmentEntityType, 1, "user-1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_LogWarning_AndNotThrow_WhenPhysicalFileDeletionFailsAsync()
    {
        var existing = MakeIntervention(1);
        var persisted = MakeIntervention(1);
        var attachment = MakeAttachment(10, 1, "interventions/1/file.pdf");

        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessment(existing));
        _attachmentRepository
            .Setup(R => R.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new[] { attachment });
        _mapper
            .Setup(M => M.Map(It.IsAny<UpdateInterventionDto>()))
            .Returns(persisted);
        SetupTransaction(persisted);
        _attachmentRepository
            .Setup(R => R.DeleteByIdsAsync(It.IsAny<int[]>()))
            .ReturnsAsync(1);
        _attachmentService
            .Setup(S => S.DeleteByStorageKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("disk error"));

        var command = new UpdateInterventionCommand(1, 1, MakeDto(), new[] { 10 }, null);

        UpdateInterventionDto result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        _logger.Verify(L => L.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<IOException>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }


    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-10")]
    public async Task HandleAsync_NewProfessional_IsAddedToAssessmentAsync()
    {
        // Arrange (GIVEN an assessment whose professionals are Ana)
        var existing = MakeIntervention(1);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessmentWith(["ana-sub"], existing));
        _mapper.Setup(M => M.Map(It.IsAny<UpdateInterventionDto>())).Returns(MakeIntervention(1));
        SetupTransaction(MakeIntervention(1));
        var command = new UpdateInterventionCommand(1, 1, MakeDto(Professional: "Luis Gómez", ProfessionalSub: "luis-sub"), null, null);

        // Act (WHEN the intervention is reassigned to Luis)
        await _handler.Handle(command, CancellationToken.None);

        // Assert (THEN Luis is added to the assessment)
        _assessmentRepository.Verify(R => R.AddAssignedProfessionalAsync(1, "luis-sub"), Times.Once);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-11")]
    public async Task HandleAsync_ProfessionalAlreadyInAssessment_IsNotDuplicatedAsync()
    {
        // Arrange (GIVEN Ana and Luis are already in the assessment)
        var existing = MakeIntervention(1);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessmentWith(["ana-sub", "luis-sub"], existing));
        _mapper.Setup(M => M.Map(It.IsAny<UpdateInterventionDto>())).Returns(MakeIntervention(1));
        SetupTransaction(MakeIntervention(1));
        var command = new UpdateInterventionCommand(1, 1, MakeDto(Professional: "Luis Gómez", ProfessionalSub: "luis-sub"), null, null);

        // Act (WHEN the intervention is reassigned to Luis)
        await _handler.Handle(command, CancellationToken.None);

        // Assert (THEN the list is not touched again)
        _assessmentRepository.Verify(R => R.AddAssignedProfessionalAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-12")]
    public async Task HandleAsync_ReassignOneIntervention_LeavesOthersAndAssessmentUnchangedAsync()
    {
        // Arrange (GIVEN two interventions in one assessment)
        var first = MakeIntervention(1);
        var second = MakeIntervention(2);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessmentWith(["ana-sub"], first, second));
        _mapper.Setup(M => M.Map(It.IsAny<UpdateInterventionDto>())).Returns(MakeIntervention(1));
        SetupTransaction(MakeIntervention(1));
        var command = new UpdateInterventionCommand(1, 1, MakeDto(Professional: "Luis Gómez", ProfessionalSub: "luis-sub"), null, null);

        // Act (WHEN only the first is reassigned)
        await _handler.Handle(command, CancellationToken.None);

        // Assert (THEN only that intervention is updated and the assessment itself is not rewritten)
        _assessmentRepository.Verify(R => R.UpdateInterventionAsync(1, It.IsAny<Intervention>()), Times.Once);
        _assessmentRepository.Verify(R => R.UpdateInterventionAsync(2, It.IsAny<Intervention>()), Times.Never);
        _assessmentRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-7")]
    public async Task HandleAsync_ChangeOnlyProfessional_KeepsOtherFieldsAsync()
    {
        // Arrange (GIVEN a handler wired with the real mapper)
        var handler = new UpdateInterventionCommandHandler(
            _attachmentService.Object,
            _attachmentRepository.Object,
            _assessmentRepository.Object,
            _unitOfWork.Object,
            _userIdentityProvider.Object,
            _logger.Object,
            new UpdateInterventionMapper(),
            _statusValidator.Object);
        _assessmentRepository
            .Setup(R => R.GetByIdWithInterventionsAsync(1))
            .ReturnsAsync(MakeAssessmentWith(["ana-sub"], MakeIntervention(1)));
        Intervention? sent = null;
        _unitOfWork
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<Intervention>>>()))
            .Returns<Func<Task<Intervention>>>(F => F());
        _assessmentRepository
            .Setup(R => R.UpdateInterventionAsync(It.IsAny<int>(), It.IsAny<Intervention>()))
            .Callback<int, Intervention>((_, I) => sent = I)
            .ReturnsAsync((int _, Intervention I) => I);
        UpdateInterventionDto dto = MakeDto(Professional: "Luis Gómez", ProfessionalSub: "luis-sub", Comments: "First session");

        // Act (WHEN only the professional changes)
        await handler.Handle(new UpdateInterventionCommand(1, 1, dto, null, null), CancellationToken.None);

        // Assert (THEN everything else matches what was sent)
        Assert.NotNull(sent);
        Assert.Equal("Luis Gómez", sent!.Professional);
        Assert.Equal("activity", sent.Activity);
        Assert.Equal("area", sent.Area);
        Assert.Equal("First session", sent.Comments);
        Assert.Equal(InterventionStatus.Remitted, sent.Status);
        Assert.Equal(dto.StudentIds, sent.StudentIds);
        Assert.Equal(dto.Attendance, sent.Attendance);
    }
}
