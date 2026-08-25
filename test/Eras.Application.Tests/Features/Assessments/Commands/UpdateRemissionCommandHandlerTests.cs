using System.ComponentModel.DataAnnotations;

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
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

    public UpdateRemissionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
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

        _handler = new UpdateRemissionCommandHandler(
            _toDomainMapper.Object,
            _toDtoMapper.Object,
            _validator.Object,
            _assessmentStatusValidator.Object,
            _interventionStatusValidator.Object,
            _mockRepository.Object,
            _mockLogger.Object);
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
}
