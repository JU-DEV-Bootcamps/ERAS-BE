using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Commands.CreatePollVariableList;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Variables.Commands;

public class CreatePollVariableListCommandHandlerTests
{
    private readonly Mock<IPollVariableRepository> _repository = new();
    private readonly Mock<ILogger<CreatePollVariableListCommandHandler>> _logger = new();

    private CreatePollVariableListCommandHandler CreateHandler() => new(_repository.Object, _logger.Object);

    [Fact]
    public async Task Handle_WhenVariableAlreadyExists_ReturnsExistingVariable_AndDoesNotAdd()
    {
        // Arrange
        const int pollId = 1;
        var existing = new Variable { Id = 10, IdPoll = pollId };

        _repository
            .Setup(x => x.GetAllWithVariablesByPollIdAsync(pollId))
            .ReturnsAsync([existing]);

        var command = new CreatePollVariableListCommand
        {
            Variables = new()
            {
                pollId = pollId,
                variables = [new Variable { Id = 10 }]
            }
        };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Entity!);
        Assert.Same(existing, result.Entity![0]);

        _repository.Verify(
            x => x.AddBatchPollVariablesAsync(It.IsAny<List<Variable>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenVariableDoesNotExist_AddsBatchAndReturnsCreatedVariables()
    {
        // Arrange
        const int pollId = 1;
        var variable = new Variable { Id = 10 };
        var created = new Variable { Id = 10, IdPoll = pollId };

        _repository
            .Setup(x => x.GetAllWithVariablesByPollIdAsync(pollId))
            .ReturnsAsync([]);

        _repository
            .Setup(x => x.AddBatchPollVariablesAsync(
                It.Is<List<Variable>>(x => x.Count == 1 && x[0] == variable && x[0].IdPoll == pollId)))
            .ReturnsAsync([created]);

        var command = new CreatePollVariableListCommand
        {
            Variables = new()
            {
                pollId = pollId,
                variables = [variable]
            }
        };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Entity!);
        Assert.Same(created, result.Entity![0]);

        _repository.Verify(
            x => x.AddBatchPollVariablesAsync(It.IsAny<List<Variable>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsErrorResponse()
    {
        // Arrange
        const int pollId = 1;

        _repository
            .Setup(x => x.GetAllWithVariablesByPollIdAsync(pollId))
            .ReturnsAsync([]);

        _repository
            .Setup(x => x.AddBatchPollVariablesAsync(It.IsAny<List<Variable>>()))
            .ThrowsAsync(new Exception("Database error"));

        var command = new CreatePollVariableListCommand
        {
            Variables = new()
            {
                pollId = pollId,
                variables = [new Variable { Id = 10 }]
            }
        };

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal("Error creating Poll Variables", result.Message);
    }
}
