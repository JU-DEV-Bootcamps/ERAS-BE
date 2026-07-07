using System.ComponentModel.DataAnnotations;

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

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
    private readonly UpdateRemissionCommandHandler _handler;
    private readonly Mock<IAssessmentRepository> _mockRepository;

    public UpdateRemissionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _mockLogger = new Mock<ILogger<UpdateRemissionCommandHandler>>();
        _validator = new Mock<IValidator<Assessment>>();
        _toDomainMapper = new Mock<IMapper<AssessmentDto, Assessment>>();
        _toDtoMapper = new Mock<IMapper<Assessment, AssessmentDto>>();
        _handler = new UpdateRemissionCommandHandler(
            _toDomainMapper.Object,
            _toDtoMapper.Object,
            _validator.Object, 
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
        var expectedDto = new AssessmentDto { 
            Id = 1, 
            StudentIds = [1, 2, 3], 
            CreatedBy = "",
            Service = "",
            Status= AssessmentStatus.Remitted,
        };

        _toDomainMapper.Setup(m => m.Map(dto)).Returns(mappedEntity);
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepository.Setup(r => r.GetByIdNoTrackingAsync(1)).ReturnsAsync(existingEntity);
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
        _mockRepository.Setup(r => r.GetByIdNoTrackingAsync(1)).ReturnsAsync(existingEntity);
        _mockRepository.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(persistedEntity);
        _toDtoMapper.Setup(m => m.Map(persistedEntity)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Status, AssessmentStatus.InProgress);
        _mockRepository.Verify(r => r.GetInterventionsContainingStudentAsync(
            It.IsAny<Assessment>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
    }
}
