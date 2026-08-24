using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Tests.Features.JUInterventions.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Interventions.Commands.CreateIntervention;
using Eras.Application.Models.Enums;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

public class CreateInterventionCommandHandlerTests
{
    private readonly Mock<IInterventionRepository> _interventionRepositoryMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IRemissionRepository> _remissionRepositoryMock;
    private readonly Mock<ILogger<CreateInterventionCommandHandler>> _loggerMock;

    private readonly CreateInterventionCommandHandler _handler;

    public CreateInterventionCommandHandlerTests()
    {
        _interventionRepositoryMock = new Mock<IInterventionRepository>();
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _remissionRepositoryMock = new Mock<IRemissionRepository>();
        _loggerMock = new Mock<ILogger<CreateInterventionCommandHandler>>();

        _handler = new CreateInterventionCommandHandler(
            _interventionRepositoryMock.Object,
            _studentRepositoryMock.Object,
            _remissionRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenInterventionIdIsZero_AddsInterventionAndReturnsSuccess()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 0,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        var createdIntervention = new JUIntervention
        {
            Id = 123
        };

        _interventionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<JUIntervention>()))
            .ReturnsAsync(createdIntervention);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Equal(createdIntervention, result.Entity);

        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()),
            Times.Once);

        _interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInterventionIdIsNonZeroAndEntityDoesNotExist_AddsIntervention()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 10,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        var createdIntervention = new JUIntervention
        {
            Id = 10
        };

        _interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(intervention.Id))
            .ReturnsAsync((JUIntervention?)null);

        _interventionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<JUIntervention>()))
            .ReturnsAsync(createdIntervention);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Equal(createdIntervention, result.Entity);

        _interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(intervention.Id),
            Times.Once);

        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInterventionAlreadyExists_ReturnsAlreadyExistsResponse()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 10,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var existingIntervention = new JUIntervention
        {
            Id = 10
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        _interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(intervention.Id))
            .ReturnsAsync(existingIntervention);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Entity already exists", result.Message);
        Assert.Equal(
            CommandEnums.CommandResultStatus.AlreadyExists,
            result.Status);

        Assert.NotNull(result.Entity);

        _interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(intervention.Id),
            Times.Once);

        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAddAsyncThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 0,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        var exception = new Exception("Database error");

        _interventionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<JUIntervention>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Error creating intervention", result.Message);
        Assert.NotNull(result.Entity);

        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenGetByIdAsyncThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 10,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        var exception = new Exception("Database error");

        _interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(intervention.Id))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Error creating intervention", result.Message);
        Assert.NotNull(result.Entity);

        _interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(intervention.Id), Times.Once);

        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAddAsyncReturnsNull_ReturnsSuccessWithNullData()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 0,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        _interventionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<JUIntervention>()))
            .ReturnsAsync((JUIntervention)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Null(result.Entity);
    }

    [Fact]
    public async Task Handle_WhenEntityAlreadyExists_DoesNotCallAddAsync()
    {
        // Arrange
        var intervention = new JUInterventionDTO
        {
            Id = 25,
            Diagnostic = "",
            Objective = "",
            StudentId = 1
        };

        var existingIntervention = new JUIntervention
        {
            Id = 25
        };

        var request = new CreateInterventionCommand
        {
            Intervention = intervention
        };

        _interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(25))
            .ReturnsAsync(existingIntervention);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIdIsZero_DoesNotCheckIfEntityExists()
    {
        // Arrange
        var request = new CreateInterventionCommand
        {
            Intervention = new JUInterventionDTO
            {
                Id = 0,
                Diagnostic = "",
                Objective = "",
                StudentId = 1
            }
        };

        _interventionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<JUIntervention>()))
            .ReturnsAsync(new JUIntervention {Id = 1});

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _interventionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<JUIntervention>()),
            Times.Once);
    }
}
