using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Dashboard.Queries.GetDashboardKpis;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Dashboard.Queries;

public class GetDashboardKpisQueryHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepository = new();
    private readonly Mock<IPollInstanceRepository> _pollInstanceRepository = new();
    private readonly Mock<IEvaluationRepository> _evaluationRepository = new();
    private readonly Mock<ILogger<GetDashboardKpisQueryHandler>> _logger = new();
    private readonly GetDashboardKpisQueryHandler _handler;

    public GetDashboardKpisQueryHandlerTests()
    {
        _studentRepository = new Mock<IStudentRepository>();
        _pollInstanceRepository = new Mock<IPollInstanceRepository>();
        _evaluationRepository = new Mock<IEvaluationRepository>();
        _logger = new Mock<ILogger<GetDashboardKpisQueryHandler>>();
        _handler = new GetDashboardKpisQueryHandler(
            _studentRepository.Object, _pollInstanceRepository.Object,
            _evaluationRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCalculatedKpis_WhenRepositoriesSucceed()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();
        var previousRange = CohortsHelper.GetPreviousCohortRange();

        _studentRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(100);

        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(30);

        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(20);

        _pollInstanceRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(200);

        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(75);

        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(50);

        _evaluationRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(80);

        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(15);

        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(20);

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal("KPIs calculated successfully", response.Message);
        Assert.NotNull(response.Body);

        Assert.Equal(100, response.Body.TotalStudents.Value);
        Assert.Equal(50.0, response.Body.TotalStudents.PercentageChange);

        Assert.Equal(200, response.Body.TotalPollsAnswered.Value);
        Assert.Equal(50.0, response.Body.TotalPollsAnswered.PercentageChange);

        Assert.Equal(80, response.Body.TotalEvaluations.Value);
        Assert.Equal(-25.0, response.Body.TotalEvaluations.PercentageChange);
    }

    [Fact]
    public async Task Handle_Returns100PercentChange_WhenPreviousCountIsZeroAndCurrentIsGreaterThanZero()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();
        var previousRange = CohortsHelper.GetPreviousCohortRange();

        _studentRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(10);

        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);

        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(0);

        _pollInstanceRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(20);

        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(5);

        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(0);

        _evaluationRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(30);

        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(0);

        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(0);

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Body);

        Assert.Equal(100.0, response.Body.TotalStudents.PercentageChange);
        Assert.Equal(100.0, response.Body.TotalPollsAnswered.PercentageChange);
        Assert.Equal(0.0, response.Body.TotalEvaluations.PercentageChange);
    }

    [Fact]
    public async Task Handle_ReturnsNegativePercentage_WhenCurrentCountIsLowerThanPrevious()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();
        var previousRange = CohortsHelper.GetPreviousCohortRange();

        _studentRepository.Setup(x => x.CountAsync()).ReturnsAsync(50);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(40);

        _pollInstanceRepository.Setup(x => x.CountAsync()).ReturnsAsync(50);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(40);

        _evaluationRepository.Setup(x => x.CountAsync()).ReturnsAsync(50);
        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);
        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(40);

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Body);

        Assert.Equal(-75.0, response.Body.TotalStudents.PercentageChange);
        Assert.Equal(-75.0, response.Body.TotalPollsAnswered.PercentageChange);
        Assert.Equal(-75.0, response.Body.TotalEvaluations.PercentageChange);
    }

    [Fact]
    public async Task Handle_RoundsPercentageChangeToTwoDecimalPlaces()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();
        var previousRange = CohortsHelper.GetPreviousCohortRange();

        _studentRepository.Setup(x => x.CountAsync()).ReturnsAsync(100);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(13);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(6);

        _pollInstanceRepository.Setup(x => x.CountAsync()).ReturnsAsync(100);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(13);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(6);

        _evaluationRepository.Setup(x => x.CountAsync()).ReturnsAsync(100);
        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(13);
        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(6);

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Body);

        Assert.Equal(116.67, response.Body.TotalStudents.PercentageChange);
        Assert.Equal(116.67, response.Body.TotalPollsAnswered.PercentageChange);
        Assert.Equal(116.67, response.Body.TotalEvaluations.PercentageChange);
    }

    [Fact]
    public async Task Handle_ReturnsZeroPercentage_WhenCurrentAndPreviousCountsAreZero()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();
        var previousRange = CohortsHelper.GetPreviousCohortRange();

        _studentRepository.Setup(x => x.CountAsync()).ReturnsAsync(0);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(0);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(0);

        _pollInstanceRepository.Setup(x => x.CountAsync()).ReturnsAsync(0);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(0);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(0);

        _evaluationRepository.Setup(x => x.CountAsync()).ReturnsAsync(0);
        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(0);
        _evaluationRepository
            .Setup(x => x.CountByDateRangeAsync(previousRange.Start, previousRange.End))
            .ReturnsAsync(0);

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Body);

        Assert.Equal(0, response.Body.TotalStudents.Value);
        Assert.Equal(0.0, response.Body.TotalStudents.PercentageChange);

        Assert.Equal(0, response.Body.TotalPollsAnswered.Value);
        Assert.Equal(0.0, response.Body.TotalPollsAnswered.PercentageChange);

        Assert.Equal(0, response.Body.TotalEvaluations.Value);
        Assert.Equal(0.0, response.Body.TotalEvaluations.PercentageChange);
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenStudentRepositoryThrows()
    {
        // Arrange
        var exception = new InvalidOperationException("Database error");

        _studentRepository
            .Setup(x => x.CountAsync())
            .ThrowsAsync(exception);

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("Error", response.Message);
        Assert.Null(response.Body);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Dashboard KPI Error")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenPollRepositoryThrows()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();

        _studentRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(100);

        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);

        _pollInstanceRepository
            .Setup(x => x.CountAsync())
            .ThrowsAsync(new Exception("Poll database failure"));

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("Error", response.Message);
        Assert.Null(response.Body);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Dashboard KPI Error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsErrorResponse_WhenEvaluationRepositoryThrows()
    {
        // Arrange
        var currentRange = CohortsHelper.GetCurrentCohortRange();

        _studentRepository.Setup(x => x.CountAsync()).ReturnsAsync(100);
        _studentRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);

        _pollInstanceRepository.Setup(x => x.CountAsync()).ReturnsAsync(100);
        _pollInstanceRepository
            .Setup(x => x.CountByDateRangeAsync(currentRange.Start, currentRange.End))
            .ReturnsAsync(10);

        _evaluationRepository
            .Setup(x => x.CountAsync())
            .ThrowsAsync(new Exception("Evaluation database failure"));

        // Act
        var response = await _handler.Handle(new GetDashboardKpisQuery(), CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("Error", response.Message);
        Assert.Null(response.Body);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Dashboard KPI Error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
