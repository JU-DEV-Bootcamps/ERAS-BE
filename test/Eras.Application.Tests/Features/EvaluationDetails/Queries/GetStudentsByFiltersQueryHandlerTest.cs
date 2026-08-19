using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.EvaluationDetails.Queries;

public class GetStudentsByFiltersQueryHandlerTest
{
    private readonly Mock<IErasEvaluationDetailsViewRepository> _repositoryMock;
    private readonly Mock<IEvaluationRepository> _evaluationRepositoryMock;
    private readonly Mock<ILogger<GetStudentsByFiltersQueryHandler>> _loggerMock;
    private readonly GetStudentsByFiltersQueryHandler _handler;

    public GetStudentsByFiltersQueryHandlerTest()
    {
        _repositoryMock = new Mock<IErasEvaluationDetailsViewRepository>();
        _evaluationRepositoryMock = new Mock<IEvaluationRepository>();
        _loggerMock = new Mock<ILogger<GetStudentsByFiltersQueryHandler>>();
        _handler = new GetStudentsByFiltersQueryHandler(
            _repositoryMock.Object,
            _evaluationRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedStudents_WhenRequestIsValid()
    {
        var evaluationId = 123;
        var startDate = new DateTime(2026, 1, 10, 8, 30, 0, DateTimeKind.Unspecified);
        var endDate = new DateTime(2026, 1, 31, 18, 45, 0, DateTimeKind.Unspecified);
        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var componentNames = new List<string>
        {
            "Mathematics",
            "Science"
        };
        var cohortIds = new List<int> { 1, 2 };
        var variableIds = new List<int> { 10, 20};
        var riskLevels = new List<decimal> { 10, 20 };

        var query = new GetStudentsByFiltersQuery
        {
            EvaluationId = evaluationId,
            PollUuid = "1234",
            ComponentNames = componentNames,
            CohortIds = cohortIds,
            VariableIds = variableIds,
            RiskLevels = riskLevels,
            PageValues = new Pagination
            {
                Page = 1,
                PageSize = 10
            }
        };

        var students = new List<ErasEvaluationDetailsView>
        {
            new ErasEvaluationDetailsView
            {
                StudentId = 1,
                StudentName = "John Doe",
                StudentEmail = "john@example.com",
                AnswerId = 100,
                AnswerText = "Answer 1",
                ComponentName = "Science",
                EvaluationName = "Test",
                Status = "Pending",       
                PollName = "Poll",
                PollUuid = "1234",
                VariableName = "Question",
                EvaluationId = evaluationId,
                CohortId = 1,
                VariableId = 20,
                RiskLevel = 10,
            },
            new ErasEvaluationDetailsView
            {
                StudentId = 2,
                StudentName = "Jane Doe",
                StudentEmail = "jane@example.com",
                AnswerId = 200,
                AnswerText = "Answer 2",
                ComponentName = "Mathematics",
                EvaluationName = "Test",
                Status = "Pending",
                PollName = "Poll",
                PollUuid = "1234",
                VariableName = "Question",
                EvaluationId = evaluationId,
                CohortId = 2,
                VariableId = 10,
                RiskLevel = 10,
            }
        };

        var totalCount = 25;
        var expectedStartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        var expectedEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByFilters(
                "1234",
                componentNames,
                cohortIds,
                variableIds,
                riskLevels,
                2,
                10,
                expectedStartDate,
                expectedEndDate,
                evaluationId))
            .ReturnsAsync(students);

        _repositoryMock
            .Setup(x => x.CountStudentsByFilters(
                "1234",
                componentNames,
                cohortIds,
                variableIds,
                riskLevels,
                expectedStartDate,
                expectedEndDate,
                evaluationId))
            .ReturnsAsync(totalCount);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(totalCount, result.Count);
        Assert.Equal(2, result.Items.Count);

        Assert.Equal(1, result.Items[0].Id);
        Assert.Equal("John Doe", result.Items[0].Name);
        Assert.Equal("john@example.com", result.Items[0].Email);
        Assert.Equal(100, result.Items[0].AnswerId);
        Assert.Equal("Answer 1", result.Items[0].AnswerText);

        Assert.Equal(2, result.Items[1].Id);
        Assert.Equal("Jane Doe", result.Items[1].Name);
        Assert.Equal("jane@example.com", result.Items[1].Email);
        Assert.Equal(200, result.Items[1].AnswerId);
        Assert.Equal("Answer 2", result.Items[1].AnswerText);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenEvaluationDoesNotExist()
    {
        var evaluationId = 999;

        var query = new GetStudentsByFiltersQuery
        {
            PollUuid = "1234",
            CohortIds = new List<int>(),
            ComponentNames = new List<string>(),
            EvaluationId = evaluationId,
            PageValues = new Pagination
            {
                Page = 1,
                PageSize = 10
            }
        };

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync((Evaluation?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);

        _evaluationRepositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);

        _repositoryMock.Verify(
            x => x.GetStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()),
            Times.Never);

        _repositoryMock.Verify(
            x => x.CountStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectFilters_ToCountRepository()
    {
        var evaluationId = 200;

        var startDate = new DateTime(2026, 3, 5);
        var endDate = new DateTime(2026, 3, 20);

        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var componentNames = new List<string> { "Math" };
        var cohortIds = new List<int> { 1 };
        var variableIds = new List<int> { 2 };
        var riskLevels = new List<decimal> { 1.2m };

        var query = new GetStudentsByFiltersQuery
        {
            EvaluationId = evaluationId,
            PollUuid = "1",
            ComponentNames = componentNames,
            CohortIds = cohortIds,
            VariableIds = variableIds,
            RiskLevels = riskLevels,
            PageValues = new Pagination
            {
                Page = 1,
                PageSize = 10
            }
        };

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()))
            .ReturnsAsync(new List<ErasEvaluationDetailsView>());

        _repositoryMock
            .Setup(x => x.CountStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()))
            .ReturnsAsync(42);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(42, result.Count);

        _repositoryMock.Verify(
            x => x.CountStudentsByFilters(
                "1",
                componentNames,
                cohortIds,
                variableIds,
                riskLevels,
                DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(endDate, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1),
                evaluationId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapStudentProperties_ToResponse()
    {
        // Arrange
        var evaluationId = 400;
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31)
        };
        var query = new GetStudentsByFiltersQuery
        {
            PollUuid = "1234",
            CohortIds = new List<int>(),
            ComponentNames = new List<string>(),
            EvaluationId = evaluationId,
            PageValues = new Pagination
            {
                Page = 1,
                PageSize = 10
            }
        };
        var student = new ErasEvaluationDetailsView
        {
            StudentId = 123,
            StudentName = "Alice Smith",
            StudentEmail = "alice@example.com",
            AnswerId = 456,
            AnswerText = "Excellent",
            ComponentName = "Component2",
            EvaluationName = "Test",
            Status = "Pending",
            PollName = "Poll",
            PollUuid = "233",
            VariableName = "Question"
        };

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                evaluationId))
            .ReturnsAsync(new List<ErasEvaluationDetailsView> { student });

        _repositoryMock
            .Setup(x => x.CountStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                evaluationId))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var response = Assert.Single(result.Items);

        Assert.Equal(student.StudentId, response.Id);
        Assert.Equal(student.StudentName, response.Name);
        Assert.Equal(student.StudentEmail, response.Email);
        Assert.Equal(student.AnswerId, response.AnswerId);
        Assert.Equal(student.AnswerText, response.AnswerText);
        Assert.Equal(student.RiskLevel, response.RiskLevel);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenStudentsRepositoryThrows()
    {
        // Arrange
        var evaluationId = 500;
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31)
        };
        var query = new GetStudentsByFiltersQuery
        {
            PollUuid = "1234",
            CohortIds = new List<int>(),
            ComponentNames = new List<string>(),
            EvaluationId = evaluationId,
            PageValues = new Pagination
            {
                Page = 1,
                PageSize = 10
            }
        };

        var exception = new Exception(
            "Error retrieving students.");

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _repositoryMock
            .Setup(x => x.GetStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                evaluationId))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);

        _repositoryMock.Verify(
            x => x.CountStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenEvaluationRepositoryThrows()
    {
        // Arrange
        var evaluationId = 700;

        var query = new GetStudentsByFiltersQuery
        {
            PollUuid = "1234",
            CohortIds = new List<int>(),
            ComponentNames = new List<string>(),
            EvaluationId = evaluationId,
            PageValues = new Pagination
            {
                Page = 1,
                PageSize = 10
            }
        };

        var exception = new Exception("Evaluation repository error.");

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);

        _repositoryMock.Verify(
            x => x.GetStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()),
            Times.Never);

        _repositoryMock.Verify(
            x => x.CountStudentsByFilters(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<decimal>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()),
            Times.Never);
    }
}
