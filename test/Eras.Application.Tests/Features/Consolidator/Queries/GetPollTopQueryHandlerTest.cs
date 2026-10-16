using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Utils;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Consolidator.Queries;

public class GetPollTopQueryHandlerTest
{
    private readonly Mock<ILogger<GetPollTopQueryHandler>> _logger;
    private readonly Mock<IPollVariableRepository> _pollVariableRepositoryMock;
    private readonly GetPollTopQueryHandler _handler;

    public GetPollTopQueryHandlerTest()
    {
        _logger = new Mock<ILogger<GetPollTopQueryHandler>>();
        _pollVariableRepositoryMock = new Mock<IPollVariableRepository>();
        _handler = new GetPollTopQueryHandler(_logger.Object, _pollVariableRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFromRepository()
    {
        // Arrange
        var pollUuid = Guid.NewGuid();
        var variableIds = new List<int> { 1, 2 };

        var pagination = new Pagination
        {
            Page = 0,
            PageSize = 10
        };

        var expectedResult = new PagedResult<ErasCalculationsByPollDTO>
        (
            Count : 1,
            Items : [new ErasCalculationsByPollDTO { 
                AnswerText = "", 
                ComponentName = "Component", 
                StudentEmail = "abby@gmail.com",
                Question = "First question"
            }]
        );

        _pollVariableRepositoryMock
            .Setup(x => x.GetByPollUuidVariableIdAsync(
                pollUuid.ToString(),
                variableIds,
                pagination,
                null))
            .ReturnsAsync(expectedResult);

        var request = new GetPollTopQuery
        {
            PollUuid = pollUuid,
            VariableIds = variableIds,
            Pagination = pagination
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Same(expectedResult, result);

        _pollVariableRepositoryMock.Verify(
            x => x.GetByPollUuidVariableIdAsync(pollUuid.ToString(), variableIds, pagination, null),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEvaluationIdIsProvided_ForwardsItToRepository()
    {
        var pollUuid = Guid.NewGuid();
        var variableIds = new List<int> { 1 };
        var pagination = new Pagination { Page = 0, PageSize = 10 };
        var expectedResult = new PagedResult<ErasCalculationsByPollDTO>(0, []);

        _pollVariableRepositoryMock
            .Setup(x => x.GetByPollUuidVariableIdAsync(pollUuid.ToString(), variableIds, pagination, 7))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(
            new GetPollTopQuery { PollUuid = pollUuid, VariableIds = variableIds, Pagination = pagination, EvaluationId = 7 },
            CancellationToken.None);

        Assert.Same(expectedResult, result);
    }
}
