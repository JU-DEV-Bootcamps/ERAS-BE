using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByEvaluationId;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.EvaluationDetails.Queries;

public class GetStudentsByEvaluationIdQueryHandlerTests
{
    private readonly Mock<IErasEvaluationDetailsViewRepository> _repositoryMock;
    private readonly Mock<IEvaluationRepository> _evaluationRepositoryMock;
    private readonly Mock<ILogger<GetStudentsByEvaluationIdQueryHandler>> _loggerMock;
    private readonly GetStudentsByEvaluationIdQueryHandler _handler;

    public GetStudentsByEvaluationIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IErasEvaluationDetailsViewRepository>();
        _evaluationRepositoryMock = new Mock<IEvaluationRepository>();
        _loggerMock = new Mock<ILogger<GetStudentsByEvaluationIdQueryHandler>>();
        _handler = new GetStudentsByEvaluationIdQueryHandler(
            _repositoryMock.Object,
            _evaluationRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnStudents_WhenEvaluationExists()
    {
        var evaluationId = 123;
        var startDate = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Unspecified);
        var endDate = new DateTime(2026, 1, 31, 18, 0, 0, DateTimeKind.Unspecified);
        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var componentNames = new List<string> { "Mathematics", "Science" };
        var cohortIds = new List<int> { 1, 2 };
        var variableIds = new List<int> { 10, 20 };
        var riskLevels = new List<decimal> { 4, 2 };

        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            ComponentNames = componentNames,
            CohortIds = cohortIds,
            VariableIds = variableIds,
            RiskLevels = riskLevels
        };
        var expectedStudents = new List<StudentsByFiltersResponse>
        {
            new StudentsByFiltersResponse(),
            new StudentsByFiltersResponse()
        };
        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);
        _repositoryMock
            .Setup(x => x.GetStudentsByEvaluationIdFilters(
                evaluationId, componentNames, cohortIds, variableIds, riskLevels,
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(endDate, DateTimeKind.Utc)))
            .ReturnsAsync(expectedStudents);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Same(expectedStudents, result);
        _evaluationRepositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);
        _repositoryMock.Verify(
            x => x.GetStudentsByEvaluationIdFilters(
                evaluationId, componentNames, cohortIds, variableIds, riskLevels,
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(endDate, DateTimeKind.Utc)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenEvaluationDoesNotExist()
    {
        var evaluationId = 999;
        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            ComponentNames = new List<string>(),
            CohortIds = new List<int>(),
            VariableIds = new List<int>(),
            RiskLevels = new List<decimal>()
        };
        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync((Evaluation?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);

        _evaluationRepositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);

        _repositoryMock.Verify(
            x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenEvaluationRepositoryThrowsException()
    {
        var evaluationId = 123;
        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            CohortIds = new List<int>(),
            ComponentNames = new List<string>()
        };
        var exception = new Exception("Database error");
        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ThrowsAsync(exception);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(
            x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenStudentRepositoryThrowsException()
    {
        var evaluationId = 123;
        var startDate = new DateTime(2026, 1, 1, 8, 0, 0);
        var endDate = new DateTime(2026, 1, 31, 18, 0, 0);
        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            ComponentNames = new List<string> { "Mathematics" },
            CohortIds = new List<int> { 1 },
            VariableIds = new List<int> { 10 },
            RiskLevels = new List<decimal> { 3 }
        };

        var exception = new Exception("Student repository error");

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ThrowsAsync(exception);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);

        _evaluationRepositoryMock.Verify(
            x => x.GetByIdAsync(evaluationId),
            Times.Once);

        _repositoryMock.Verify(
            x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectFiltersToRepository()
    {
        var evaluationId = 456;
        var startDate = new DateTime(2026, 2, 10, 9, 30, 0);
        var endDate = new DateTime(2026, 2, 20, 17, 30, 0);

        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var componentNames = new List<string>
        {
            "Component A",
            "Component B"
        };

        var cohortIds = new List<int> { 10, 20, 30 };
        var variableIds = new List<int> { 100, 200 };
        var riskLevels = new List<decimal> { 1, 2 };

        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            ComponentNames = componentNames,
            CohortIds = cohortIds,
            VariableIds = variableIds,
            RiskLevels = riskLevels
        };

        var expectedResult =
            new List<StudentsByFiltersResponse>();

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(expectedResult);

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(
            x => x.GetStudentsByEvaluationIdFilters(
                evaluationId,
                componentNames,
                cohortIds,
                variableIds,
                riskLevels,
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(endDate, DateTimeKind.Utc)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldConvertStartAndEndDatesToUtc()
    {
        var evaluationId = 123;
        var startDate = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Local);
        var endDate = new DateTime(2026, 3, 31, 20, 0, 0, DateTimeKind.Local);
        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            CohortIds = new List<int>(),
            ComponentNames = new List<string>()
        };

        var expectedResult =
            new List<StudentsByFiltersResponse>();

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(expectedResult);

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(
            x => x.GetStudentsByEvaluationIdFilters(
                evaluationId,
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.Is<DateTime>(d => d == startDate && d.Kind == DateTimeKind.Utc),
                It.Is<DateTime>(d => d == endDate && d.Kind == DateTimeKind.Utc)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
    {
        var evaluationId = 123;
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31)
        };

        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            CohortIds = new List<int>(),
            ComponentNames = new List<string>()
        };

        var expectedResult = new List<StudentsByFiltersResponse>();

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByEvaluationIdFilters(
                It.IsAny<int>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        var evaluationId = 123;
        var exception = new Exception("Something went wrong");

        var query = new GetStudentsByEvaluationIdQuery
        {
            EvaluationId = evaluationId,
            CohortIds = new List<int>(),
            ComponentNames = new List<string>()
        };

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ThrowsAsync(exception);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) =>
                    state.ToString()!.Contains("An error occurred while filtering students by evaluation id")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
