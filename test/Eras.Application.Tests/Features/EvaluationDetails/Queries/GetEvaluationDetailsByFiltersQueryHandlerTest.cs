using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetEvaluationDetailsByFilters;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.EvaluationDetails.Queries;

public class GetEvaluationDetailsByFiltersQueryHandlerTest
{
    private readonly Mock<IErasEvaluationDetailsViewRepository> _repositoryMock;
    private readonly Mock<ILogger<GetEvaluationDetailsByFiltersQueryHandler>> _loggerMock;
    private readonly GetEvaluationDetailsByFiltersQueryHandler _handler;

    public GetEvaluationDetailsByFiltersQueryHandlerTest()
    {
        _repositoryMock = new Mock<IErasEvaluationDetailsViewRepository>();
        _loggerMock = new Mock<ILogger<GetEvaluationDetailsByFiltersQueryHandler>>();

        _handler = new GetEvaluationDetailsByFiltersQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithAllFilters()
    {
        var pollId = 123;
        var componentIds = new List<int> { 1, 2, 3 };
        var cohortIds = new List<int> { 10, 20 };
        var variableIds = new List<int> { 100, 200 };

        var query = new GetEvaluationDetailsByFiltersQuery
        {
            PollId = pollId,
            ComponentIds = componentIds,
            CohortIds = cohortIds,
            VariableIds = variableIds
        };

        var expectedResult = new List<ErasEvaluationDetailsView>
        {
            new ErasEvaluationDetailsView()
            {
                ComponentName = "Component1",
                EvaluationName = "Test1",
                Status = "Pending",
                StudentEmail = "atest@mail.com",
                StudentName = "Abby",
                AnswerText = "Test",
                PollName = "Poll",
                PollUuid = "233",
                VariableName = "Question"
            },
            new ErasEvaluationDetailsView()
            {
                ComponentName = "Component2",
                EvaluationName = "Test",
                Status = "Pending",
                StudentEmail = "btest@mail.com",
                StudentName = "Bri",
                AnswerText = "Test",
                PollName = "Poll",
                PollUuid = "233",
                VariableName = "Question"
            }
        };

        _repositoryMock
            .Setup(x => x.GetByFiltersAsync(
                pollId,
                componentIds,
                cohortIds,
                variableIds))
            .ReturnsAsync(expectedResult);
        var result = await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(
            x => x.GetByFiltersAsync(pollId, componentIds, cohortIds,variableIds), Times.Once);
        Assert.Same(expectedResult, result);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
    {
        var query = new GetEvaluationDetailsByFiltersQuery
        {
            PollId = 123,
            ComponentIds = new List<int> { 1, 2 },
            CohortIds = new List<int> { 10 },
            VariableIds = new List<int> { 100 }
        };
        var expectedResult = new List<ErasEvaluationDetailsView>();
        _repositoryMock
            .Setup(x => x.GetByFiltersAsync(query.PollId, query.ComponentIds, query.CohortIds, query.VariableIds))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(
            x => x.GetByFiltersAsync(query.PollId, query.ComponentIds, query.CohortIds, query.VariableIds), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnRepositoryResult_Unchanged()
    {
        var query = new GetEvaluationDetailsByFiltersQuery
        {
            PollId = 456,
            ComponentIds = new List<int> { 5 },
            CohortIds = new List<int> { 15 },
            VariableIds = new List<int> { 25 }
        };
        var expectedResult = new List<ErasEvaluationDetailsView>
        {
            new ErasEvaluationDetailsView()
            {
                ComponentName = "Component2",
                EvaluationName = "Test",
                Status = "Pending",
                StudentEmail = "test@mail.com",
                StudentName = "Abby",
                AnswerText = "Test",
                PollName = "Poll",
                PollUuid = "233",
                VariableName = "Question"
            }
        };

        _repositoryMock
            .Setup(x => x.GetByFiltersAsync(query.PollId, query.ComponentIds, query.CohortIds, query.VariableIds))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Same(expectedResult, result);
    }

    [Fact]
    public async Task Handle_ShouldPropagateRepositoryException()
    {
        var query = new GetEvaluationDetailsByFiltersQuery
        {
            PollId = 123,
            ComponentIds = new List<int> { 1 },
            CohortIds = new List<int> { 2 },
            VariableIds = new List<int> { 3 }
        };
        var expectedException = new Exception("Repository error");
        _repositoryMock
            .Setup(x => x.GetByFiltersAsync( query.PollId, query.ComponentIds, query.CohortIds, query.VariableIds))
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(query, CancellationToken.None));

        Assert.Same(expectedException, exception);

        _repositoryMock.Verify(
            x => x.GetByFiltersAsync(query.PollId, query.ComponentIds, query.CohortIds, query.VariableIds),Times.Once);
    }
}
