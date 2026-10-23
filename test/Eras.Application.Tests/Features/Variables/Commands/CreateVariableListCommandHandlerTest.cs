using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Variables.Commands.CreateVariableList;
using Eras.Application.Models.CommandsDTOS;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Variables.Commands;

public class CreateVariableListCommandHandlerTests
{
    private readonly Mock<IVariableRepository> _repository = new();
    private readonly Mock<ILogger<CreateVariableListCommandHandler>> _logger = new();

    private CreateVariableListCommandHandler CreateHandler() =>
        new(_repository.Object, _logger.Object);

    [Fact]
    public async Task Handle_WhenVariableAlreadyExists_ReturnsExistingVariable_AndDoesNotAdd()
    {
        // Arrange
        const int pollId = 1;

        var existing = new Variable
        {
            Id = 10,
            IdPoll = pollId,
            Name = "Age",
            Position = 1
        };

        _repository
            .Setup(x => x.GetByPollIdAsync(pollId))
            .ReturnsAsync([existing]);

        var command = new CreateVariableListCommand
        {
            Variables =
            [
                new VariableListCommandDTO
                {
                    PollId = pollId,
                    ComponentId = 5,
                    variable = new VariableDTO
                    {
                        Name = "Age",
                        Position = 1
                    }
                }
            ]
        };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Same(existing, result.Entity![0]);

        _repository.Verify(
            x => x.AddTrackedBatchAsync(It.IsAny<List<Variable>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenVariableDoesNotExist_AddsVariableWithPollAndComponentIds()
    {
        // Arrange
        const int pollId = 1;
        const int componentId = 5;

        _repository
            .Setup(x => x.GetByPollIdAsync(pollId))
            .ReturnsAsync([]);

        var created = new Variable
        {
            Id = 10,
            IdPoll = pollId,
            IdComponent = componentId,
            Name = "Age",
            Position = 1
        };

        _repository
            .Setup(x => x.AddTrackedBatchAsync(
                It.Is<List<Variable>>(variables =>
                    variables.Count == 1 && variables[0].IdPoll == pollId && variables[0].IdComponent == componentId)))
            .ReturnsAsync([created]);

        var command = new CreateVariableListCommand
        {
            Variables =
            [
                new VariableListCommandDTO
                {
                    PollId = pollId,
                    ComponentId = componentId,
                    variable = new VariableDTO
                    {
                        Name = "Age",
                        Position = 1
                    }
                }
            ]
        };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Same(created, result.Entity![0]);

        _repository.Verify(
            x => x.AddTrackedBatchAsync(It.IsAny<List<Variable>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsErrorResponse()
    {
        // Arrange
        const int pollId = 1;

        _repository
            .Setup(x => x.GetByPollIdAsync(pollId))
            .ReturnsAsync([]);

        _repository
            .Setup(x => x.AddTrackedBatchAsync(It.IsAny<List<Variable>>()))
            .ThrowsAsync(new Exception("Database error"));

        var command = new CreateVariableListCommand
        {
            Variables =
            [
                new VariableListCommandDTO
                {
                    PollId = pollId,
                    ComponentId = 5,
                    variable = new VariableDTO
                    {
                        Name = "Age",
                        Position = 1
                    }
                }
            ]
        };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Null(result.Entity);
        Assert.Equal("Error creating variables", result.Message);
    }
}
