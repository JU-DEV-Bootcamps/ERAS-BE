using System.ComponentModel.DataAnnotations;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public class UpdateRemissionCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateRemissionCommandHandler>> _mockLogger;
    private readonly Mock<IMapper<AssessmentDto, Assessment>> _toDomainMapper;
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _toDtoMapper;
    private readonly Mock<IValidator<Assessment>> _validator;
    private readonly Mock<IValidator<StatusTransitionRequest<AssessmentStatus>>> _assessmentStatusValidator;
    private readonly Mock<IValidator<StatusTransitionRequest<InterventionStatus>>> _interventionStatusValidator;
    private readonly UpdateRemissionCommandHandler _handler;
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IErasUsersRepository> _usersRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICurrentUserService> _currentUser;
    private readonly Mock<IAssessmentActivityRepository> _activityRepository;
    private readonly List<AssessmentActivity> _recordedActivities = new();

    public UpdateRemissionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _usersRepository = new Mock<IErasUsersRepository>();
        _mockLogger = new Mock<ILogger<UpdateRemissionCommandHandler>>();
        _validator = new Mock<IValidator<Assessment>>();
        _assessmentStatusValidator = new Mock<IValidator<StatusTransitionRequest<AssessmentStatus>>>();
        _interventionStatusValidator = new Mock<IValidator<StatusTransitionRequest<InterventionStatus>>>();
        _toDomainMapper = new Mock<IMapper<AssessmentDto, Assessment>>();
        _toDtoMapper = new Mock<IMapper<Assessment, AssessmentDto>>();

        // Default: both status transition validators pass, unless a specific test overrides this.
        _assessmentStatusValidator
            .Setup(v => v.ValidateAsync(It.IsAny<StatusTransitionRequest<AssessmentStatus>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _interventionStatusValidator
            .Setup(v => v.ValidateAsync(It.IsAny<StatusTransitionRequest<InterventionStatus>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<Assessment>>>()))
            .Returns<Func<Task<Assessment>>>(F => F());
        _currentUser = new Mock<ICurrentUserService>();
        _currentUser.Setup(U => U.Sub).Returns("user-1");
        _currentUser.Setup(U => U.Name).Returns("Luis Gómez");
        _activityRepository = new Mock<IAssessmentActivityRepository>();
        _activityRepository
            .Setup(R => R.AddRangeAsync(It.IsAny<IReadOnlyCollection<AssessmentActivity>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<AssessmentActivity>, CancellationToken>((Items, _) => _recordedActivities.AddRange(Items))
            .Returns(Task.CompletedTask);

        _handler = new UpdateRemissionCommandHandler(
            _toDomainMapper.Object,
            _toDtoMapper.Object,
            _validator.Object,
            _assessmentStatusValidator.Object,
            _interventionStatusValidator.Object,
            _mockRepository.Object,
            _mockLogger.Object,
            _usersRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            _activityRepository.Object);
    }

    [Fact]
    public async Task HandleUpdateRemissionAsync()
    {
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2, 3],
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
        };
        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2, 3]
        };

        var existingEntity = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2]
        };

        var persistedEntity = mappedEntity;
        var expectedDto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2, 3],
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
        };

        _toDomainMapper.Setup(m => m.Map(dto)).Returns(mappedEntity);
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepository
            .Setup(r => r.GetByIdNoTrackingAsync(1, It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync(existingEntity);
        _mockRepository.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(persistedEntity);
        _toDtoMapper.Setup(m => m.Map(persistedEntity)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        _mockRepository.Verify(r => r.GetInterventionsContainingStudentAsync(
            It.IsAny<Assessment>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
        _assessmentStatusValidator.Verify(
            v => v.ValidateAsync(
                It.Is<StatusTransitionRequest<AssessmentStatus>>(x =>
                    x.CurrentStatus == AssessmentStatus.Remitted && x.NewStatus == AssessmentStatus.Remitted),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleUpdateRemissionStatusAsync()
    {
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2],
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.InProgress,
        };
        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.InProgress,
            StudentIds = [1, 2]
        };

        var existingEntity = new Assessment
        {
            Id = 1,
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2]
        };

        var persistedEntity = mappedEntity;
        var expectedDto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2],
            CreatedBy = "",
            Service = "",
            Status = AssessmentStatus.InProgress,
        };

        _toDomainMapper.Setup(m => m.Map(dto)).Returns(mappedEntity);
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepository
            .Setup(r => r.GetByIdNoTrackingAsync(1, It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync(existingEntity);
        _mockRepository.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(persistedEntity);
        _toDtoMapper.Setup(m => m.Map(persistedEntity)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(AssessmentStatus.InProgress, expectedDto.Status);
        _mockRepository.Verify(r => r.GetInterventionsContainingStudentAsync(
            It.IsAny<Assessment>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
        _assessmentStatusValidator.Verify(
            v => v.ValidateAsync(
                It.Is<StatusTransitionRequest<AssessmentStatus>>(x =>
                    x.CurrentStatus == AssessmentStatus.Remitted && x.NewStatus == AssessmentStatus.InProgress),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAssessmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            CreatedBy = "someone",
            Service = "workshop"
        };

        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            CreatedBy = "someone",
            Service = "workshop"
        };

        _toDomainMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedEntity);

        _validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<Assessment>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository
            .Setup(r => r.GetByIdNoTrackingAsync(1, It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync((Assessment?)null);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal("Assessment '1' not found.", exception.Message);

        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAssessmentValidationFails_ThrowsOperationCanceledException()
    {
        // Arrange
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        _toDomainMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedEntity);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(
            [
                new ValidationFailure("Status", "Assessment is invalid.")
            ]));

        // Act
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Contains("Error updating assessment", exception.Message);
        Assert.Contains("Assessment is invalid.", exception.Message);

        _mockRepository.Verify(
            r => r.GetByIdNoTrackingAsync(It.IsAny<int>(), It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenStudentsAreRemovedWithoutRelatedInterventions_UpdatesAssessment()
    {
        // Arrange
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        var existingEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1, 2, 3],
            Status = AssessmentStatus.InProgress,
            Interventions = [],
            Service = "training",
            CreatedBy = "someone"
        };

        _toDomainMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedEntity);

        _validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<Assessment>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository
            .Setup(r => r.GetByIdNoTrackingAsync(
                1,
                It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync(existingEntity);

        _mockRepository
            .Setup(r => r.GetInterventionsContainingStudentAsync(
                existingEntity,
                It.Is<IReadOnlyCollection<int>>(ids =>
                    ids.SequenceEqual(new[] { 3 }))))
            .ReturnsAsync(Array.Empty<Intervention>());

        _mockRepository
            .Setup(r => r.UpdateAsync(mappedEntity))
            .ReturnsAsync(mappedEntity);

        var expectedDto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        _toDtoMapper
            .Setup(m => m.Map(mappedEntity))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Same(expectedDto, result);

        _mockRepository.Verify(
            r => r.GetInterventionsContainingStudentAsync(
                existingEntity, It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 3 }))),
            Times.Once);

        _mockRepository.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRemovedStudentHasRelatedIntervention_ThrowsOperationCanceledException()
    {
        // Arrange
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1],
            Status = AssessmentStatus.InProgress,
            Service = "training",
            CreatedBy = "someone"
        };

        var existingEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Interventions = [],
            Service = "training",
            CreatedBy = "someone"
        };

        var intervention = new IndividualIntervention
        {
            Id = 10,
            StudentIds = [2],
            DateUtc = DateTime.UtcNow,
        };

        _toDomainMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedEntity);

        _validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository
            .Setup(r => r.GetByIdNoTrackingAsync(
                1, It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync(existingEntity);

        _mockRepository
            .Setup(r => r.GetInterventionsContainingStudentAsync(
                existingEntity,
                It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 2 }))))
            .ReturnsAsync(new[] { intervention });

        // Act
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Contains("Some students cannot be removed", exception.Message);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInterventionIsNew_DoesNotValidateInterventionStatus()
    {
        // Arrange
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [1],
            Status = AssessmentStatus.InProgress,
            Interventions =
            [
                new IndividualInterventionDto
                {
                    Id = 999,
                    Status = InterventionStatus.InProgress,
                    DateUtc = DateTime.UtcNow,
                    StudentIds = [1],
                }
            ],
            CreatedBy = "me",
            Service = "NewService"
        };

        var command = new UpdateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1],
            Status = AssessmentStatus.InProgress,
            Interventions =
            [
                new IndividualIntervention
                {
                    Id = 999,
                    Status = InterventionStatus.InProgress,
                    DateUtc = DateTime.UtcNow,
                    StudentIds = [1]
                }
            ],
            CreatedBy = "admin",
            Service = "NewService"
        };

        var existingEntity = new Assessment
        {
            Id = 1,
            StudentIds = [1],
            Status = AssessmentStatus.InProgress,
            Interventions = [],
            Service = "training",
            CreatedBy = "default"
        };

        _toDomainMapper
            .Setup(m => m.Map(dto))
            .Returns(mappedEntity);

        _validator
            .Setup(v => v.ValidateAsync(
                It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository
            .Setup(r => r.GetByIdNoTrackingAsync(
                1, It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync(existingEntity);

        _mockRepository
            .Setup(r => r.UpdateAsync(mappedEntity))
            .ReturnsAsync(mappedEntity);

        _toDtoMapper
            .Setup(m => m.Map(mappedEntity))
            .Returns(dto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _interventionStatusValidator.Verify(
            v => v.ValidateAsync(
                It.IsAny<ValidationContext<StatusTransitionRequest<InterventionStatus>>>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _mockRepository.Verify(
            r => r.UpdateAsync(mappedEntity), Times.Once);
    }


    private static Assessment MakeAssessment(string[] Professionals, params Intervention[] Interventions) => new()
    {
        Id = 1,
        CreatedBy = "Any",
        Service = "Smth",
        Status = AssessmentStatus.Remitted,
        StudentIds = [1],
        AssignedProfessionals = Professionals,
        Interventions = Interventions.ToList()
    };

    private static Intervention MakeIntervention(int Id, string? Professional) => new()
    {
        Id = Id,
        DateUtc = DateTime.UtcNow,
        StudentIds = [1],
        Professional = Professional
    };

    private static ErasUserDTO MakeUser(string Sub, string FirstName, string LastName) => new()
    {
        Sub = Sub,
        Email = Sub + "@example.com",
        FirstName = FirstName,
        LastName = LastName,
        Role = "ERAS Professional",
        Audit = new Eras.Domain.Common.AuditInfo()
    };

    private void SetupUpdate(Assessment Existing, Assessment Incoming)
    {
        var dto = new AssessmentDto { Id = 1, StudentIds = [1], CreatedBy = "", Service = "", Status = AssessmentStatus.Remitted };
        _toDomainMapper.Setup(M => M.Map(It.IsAny<AssessmentDto>())).Returns(Incoming);
        _toDtoMapper.Setup(M => M.Map(It.IsAny<Assessment>())).Returns(dto);
        _validator
            .Setup(V => V.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepository
            .Setup(R => R.GetByIdNoTrackingAsync(1, It.IsAny<Func<IQueryable<Assessment>, IQueryable<Assessment>>>()))
            .ReturnsAsync(Existing);
        _mockRepository.Setup(R => R.UpdateAsync(It.IsAny<Assessment>())).ReturnsAsync((Assessment A) => A);
        _usersRepository.Setup(R => R.GetErasUserBySubAsync("luis-sub")).ReturnsAsync(MakeUser("luis-sub", "Luis", "Gómez"));
        _usersRepository.Setup(R => R.GetErasUserBySubAsync("ana-sub")).ReturnsAsync(MakeUser("ana-sub", "Ana", "Pérez"));
    }

    private static UpdateRemissionCommand Command() =>
        new(new AssessmentDto { Id = 1, StudentIds = [1], CreatedBy = "", Service = "", Status = AssessmentStatus.Remitted });

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-16")]
    public async Task HandleAsync_ProfessionalAdded_PersistsFullListAsync()
    {
        // Arrange (GIVEN an assessment whose professionals are Ana)
        SetupUpdate(MakeAssessment(["ana-sub"]), MakeAssessment(["ana-sub", "luis-sub"]));

        // Act (WHEN Luis is added in the assessment form)
        await _handler.Handle(Command(), CancellationToken.None);

        // Assert (THEN the full list is persisted)
        _mockRepository.Verify(R => R.UpdateAsync(It.Is<Assessment>(A =>
            A.AssignedProfessionals.SequenceEqual(new[] { "ana-sub", "luis-sub" }))), Times.Once);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-17")]
    public async Task HandleAsync_RemoveProfessionalWithoutInterventions_SucceedsAsync()
    {
        // Arrange (GIVEN Luis has no intervention in the assessment)
        SetupUpdate(
            MakeAssessment(["ana-sub", "luis-sub"], MakeIntervention(10, "Ana Pérez")),
            MakeAssessment(["ana-sub"], MakeIntervention(10, "Ana Pérez")));

        // Act (WHEN Luis is removed)
        await _handler.Handle(Command(), CancellationToken.None);

        // Assert (THEN only Ana remains and the interventions are unchanged)
        _mockRepository.Verify(R => R.UpdateAsync(It.Is<Assessment>(A =>
            A.AssignedProfessionals.SequenceEqual(new[] { "ana-sub" })
            && A.Interventions.Single().Professional == "Ana Pérez")), Times.Once);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-18")]
    public async Task HandleAsync_RemoveProfessionalWithInterventions_Throws409AndKeepsListAsync()
    {
        // Arrange (GIVEN an intervention of the assessment is assigned to Luis)
        SetupUpdate(
            MakeAssessment(["ana-sub", "luis-sub"], MakeIntervention(10, "Luis Gómez")),
            MakeAssessment(["ana-sub"], MakeIntervention(10, "Luis Gómez")));

        // Act (WHEN Luis is removed)
        OperationCanceledException ex = await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(Command(), CancellationToken.None));

        // Assert (THEN the save is rejected with a message naming Luis, and nothing is persisted)
        Assert.Contains("Luis Gómez", ex.Message);
        _mockRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-8")]
    public async Task HandleAsync_NoProfessionalChange_LeavesInterventionProfessionalsAsync()
    {
        // Arrange (GIVEN existing interventions and no change to the professionals)
        SetupUpdate(
            MakeAssessment(["ana-sub"], MakeIntervention(10, "Ana Pérez")),
            MakeAssessment(["ana-sub"], MakeIntervention(10, "Ana Pérez")));

        // Act (WHEN the assessment is saved)
        await _handler.Handle(Command(), CancellationToken.None);

        // Assert (THEN each intervention keeps its professional and nobody is looked up)
        _mockRepository.Verify(R => R.UpdateAsync(It.Is<Assessment>(A =>
            A.Interventions.Single().Professional == "Ana Pérez")), Times.Once);
        _usersRepository.Verify(R => R.GetErasUserBySubAsync(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("luis-sub")]
    [InlineData("luis-sub@example.com")]
    [InlineData("  luis gómez ")]
    public async Task HandleAsync_RemoveProfessionalStoredInAnotherForm_Throws409Async(string StoredProfessional)
    {
        SetupUpdate(
            MakeAssessment(["ana-sub", "luis-sub"], MakeIntervention(10, StoredProfessional)),
            MakeAssessment(["ana-sub"], MakeIntervention(10, StoredProfessional)));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(Command(), CancellationToken.None));

        _mockRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_RemoveUnresolvableProfessionalWithInterventions_Throws409Async()
    {
        SetupUpdate(
            MakeAssessment(["ana-sub", "luis-sub"], MakeIntervention(10, "luis-sub")),
            MakeAssessment(["ana-sub"], MakeIntervention(10, "luis-sub")));
        _usersRepository.Setup(R => R.GetErasUserBySubAsync("luis-sub")).ReturnsAsync((ErasUserDTO?)null);

        OperationCanceledException ex = await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(Command(), CancellationToken.None));

        Assert.Contains("luis-sub", ex.Message);
        _mockRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_RemoveUnresolvableProfessionalWithoutInterventions_SucceedsAsync()
    {
        SetupUpdate(
            MakeAssessment(["ana-sub", "luis-sub"], MakeIntervention(10, "Ana Pérez")),
            MakeAssessment(["ana-sub"], MakeIntervention(10, "Ana Pérez")));
        _usersRepository.Setup(R => R.GetErasUserBySubAsync("luis-sub")).ReturnsAsync((ErasUserDTO?)null);

        await _handler.Handle(Command(), CancellationToken.None);

        _mockRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Once);
    }

    private static Assessment MakeTracked(AssessmentStatus Status, string? Comments) => new()
    {
        Id = 1,
        CreatedBy = "Any",
        Service = "Smth",
        Status = Status,
        Comments = Comments,
        StudentIds = [1]
    };

    private async Task SaveAsync(
        AssessmentStatus PreviousStatus, string? PreviousComments, AssessmentStatus NewStatus, string? NewComments)
    {
        SetupUpdate(MakeTracked(PreviousStatus, PreviousComments), MakeTracked(NewStatus, NewComments));
        await _handler.Handle(
            new UpdateRemissionCommand(
                new AssessmentDto { Id = 1, StudentIds = [1], CreatedBy = "", Service = "", Status = NewStatus, Comments = NewComments }),
            CancellationToken.None);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-2")]
    public async Task HandleAsync_CommentAdded_RecordsCommentAddedEventAsync()
    {
        // Arrange (GIVEN) an assessment with no comments and a user named "Luis Gómez"
        // Act (WHEN) they save the comments "Family contacted"
        await SaveAsync(AssessmentStatus.Remitted, null, AssessmentStatus.Remitted, "Family contacted");

        // Assert (THEN) one "Comment added" event with the user name
        AssessmentActivity activity = Assert.Single(_recordedActivities);
        Assert.Equal(AssessmentActivityType.CommentAdded, activity.Type);
        Assert.Equal(1, activity.AssessmentId);
        Assert.Equal("Luis Gómez", activity.ActorName);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-3")]
    public async Task HandleAsync_CommentChangedThenCleared_RecordsOnlyTheChangeAsync()
    {
        // Arrange (GIVEN) an assessment with the comments "Family contacted"
        // Act (WHEN) the user changes the comments to "Family met", then clears them
        await SaveAsync(AssessmentStatus.Remitted, "Family contacted", AssessmentStatus.Remitted, "Family met");
        await SaveAsync(AssessmentStatus.Remitted, "Family met", AssessmentStatus.Remitted, null);

        // Assert (THEN) only the change recorded an event
        AssessmentActivity activity = Assert.Single(_recordedActivities);
        Assert.Equal(AssessmentActivityType.CommentAdded, activity.Type);
        Assert.Equal("Luis Gómez", activity.ActorName);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-4")]
    public async Task HandleAsync_StatusChanged_RecordsPreviousAndNewStatusAsync()
    {
        // Arrange (GIVEN) an assessment with status Remitted
        // Act (WHEN) the user changes it to InProgress, then to Finalized
        await SaveAsync(AssessmentStatus.Remitted, null, AssessmentStatus.InProgress, null);
        await SaveAsync(AssessmentStatus.InProgress, null, AssessmentStatus.Finalized, null);

        // Assert (THEN) two status events with the previous and the new value
        Assert.Equal(2, _recordedActivities.Count);
        Assert.All(_recordedActivities, A => Assert.Equal(AssessmentActivityType.StatusChanged, A.Type));
        Assert.Equal(AssessmentStatus.Remitted, _recordedActivities[0].PreviousStatus);
        Assert.Equal(AssessmentStatus.InProgress, _recordedActivities[0].NewStatus);
        Assert.Equal(AssessmentStatus.InProgress, _recordedActivities[1].PreviousStatus);
        Assert.Equal(AssessmentStatus.Finalized, _recordedActivities[1].NewStatus);
        Assert.Equal("Luis Gómez", _recordedActivities[0].ActorName);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-5")]
    public async Task HandleAsync_NoTrackedChange_RecordsNoEventAsync()
    {
        // Arrange (GIVEN) status Remitted and comments "Family contacted"
        // Act (WHEN) the user saves keeping the same status and comments
        await SaveAsync(AssessmentStatus.Remitted, "Family contacted", AssessmentStatus.Remitted, "Family contacted");

        // Assert (THEN) the assessment is saved and no event is recorded
        _mockRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Once);
        Assert.Empty(_recordedActivities);
    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-6")]
    public async Task HandleAsync_RejectedStatusTransition_RecordsNoEventAsync()
    {
        // Arrange (GIVEN) a Finalized assessment and a validator that rejects the transition
        SetupUpdate(MakeTracked(AssessmentStatus.Finalized, null), MakeTracked(AssessmentStatus.InProgress, null));
        _assessmentStatusValidator
            .Setup(V => V.ValidateAsync(It.IsAny<StatusTransitionRequest<AssessmentStatus>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult([new ValidationFailure("Status", "Finalized is final")]));

        // Act (WHEN) a user tries to change it to InProgress
        Func<Task> act = () => _handler.Handle(
            new UpdateRemissionCommand(
                new AssessmentDto { Id = 1, StudentIds = [1], CreatedBy = "", Service = "", Status = AssessmentStatus.InProgress }),
            CancellationToken.None);

        // Assert (THEN) the existing rejection happens, nothing is saved and no event is recorded
        await Assert.ThrowsAsync<OperationCanceledException>(act);
        _mockRepository.Verify(R => R.UpdateAsync(It.IsAny<Assessment>()), Times.Never);
        Assert.Empty(_recordedActivities);
    }
}
