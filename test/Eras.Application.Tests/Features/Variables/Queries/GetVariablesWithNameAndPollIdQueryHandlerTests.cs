using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Queries.GetWithNameAndPollId;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Variables.Queries;

public class GetVariablesWithNameAndPollIdQueryHandlerTests
{
    private readonly Mock<IVariableRepository> _repository = new();
    private readonly Mock<ILogger<GetVariablesWithNameAndPollIdQueryHandler>> _logger = new();

    private GetVariablesWithNameAndPollIdQueryHandler CreateHandler() =>
        new(_repository.Object, _logger.Object);

    [Fact]
    public async Task Handle_WhenPollIdIsPositive_UsesGetByPollId()
    {
        // Arrange
        const int pollId = 1;
        var variables = new List<Variable>
        {
            new() { Id = 10, IdPoll = pollId, Name = "Age" }
        };

        _repository
            .Setup(x => x.GetByPollIdAsync(pollId))
            .ReturnsAsync(variables);

        var request = new GetVariablesWithNameAndPollIdQuery
        {
            PollId = pollId
        };

        // Act
        var result = await CreateHandler().Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Variables Found", result.Message);
        Assert.Same(variables, result.Body);

        _repository.Verify(x => x.GetByPollIdAsync(pollId), Times.Once);
        _repository.Verify(x => x.GetAllWithNameAndPollIdAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPollIdIsZero_UsesGetAllWithNameAndPollId()
    {
        // Arrange
        var variables = new List<Variable>
        {
            new() { Id = 10, Name = "Age" }
        };

        _repository
            .Setup(x => x.GetAllWithNameAndPollIdAsync())
            .ReturnsAsync(variables);

        var request = new GetVariablesWithNameAndPollIdQuery
        {
            PollId = 0
        };

        // Act
        var result = await CreateHandler().Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Variables Found", result.Message);
        Assert.Same(variables, result.Body);

        _repository.Verify(x => x.GetAllWithNameAndPollIdAsync(), Times.Once);
        _repository.Verify(x => x.GetByPollIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoVariablesFound_ReturnsNotFound()
    {
        // Arrange
        _repository
            .Setup(x => x.GetByPollIdAsync(1))
            .ReturnsAsync([]);

        var request = new GetVariablesWithNameAndPollIdQuery
        {
            PollId = 1
        };

        // Act
        var result = await CreateHandler().Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Variables Not Found", result.Message);
        Assert.Equal(Models.Enums.QueryEnums.QueryResultStatus.NotFound, result.Status);
        Assert.Empty(result.Body!);
    }
}
