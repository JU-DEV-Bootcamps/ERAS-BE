using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByEvaluationId;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsRecentAlerts;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class EvaluationDetailsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<EvaluationsController>> _mockLogger;
    private readonly EvaluationDetailsController _controller;

    public EvaluationDetailsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<EvaluationsController>>();
        _controller = new EvaluationDetailsController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetRecentAlertsOfStudentsAsync_Should_Return_SuccessAsync()
    {
        // Arrange
        var pagination = new Pagination();
        var expectedResult = new PagedResult<GetStudentsRecentAlertsResponse>(2, new List<GetStudentsRecentAlertsResponse>
        {
            new() { StudentId = "1", StudentName = "Student 1", Category = "Academic", Date = DateTime.UtcNow, Status = "Active" },
            new() { StudentId = "2", StudentName = "Student 2", Category = "Academic", Date = DateTime.UtcNow, Status = "Active" }
        });
        _mockMediator
            .Setup(X => X.Send(It.IsAny<GetStudentsRecentAlertsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetRecentAlertsOfStudentsAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<GetStudentsRecentAlertsResponse>>(okResult.Value);
        Assert.Equal(expectedResult, value);

        _mockMediator.Verify(X => X.Send(
            It.IsAny<GetStudentsRecentAlertsQuery>(),
            It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task StudentsByFilterAsync_Should_Return_SuccessAsync()
    {
        // Arrange
        var pagination = new Pagination();
        var expectedResult = new PagedResult<StudentsByFiltersResponse>(2, new List<StudentsByFiltersResponse> { new(), new() });
        _mockMediator
            .Setup(X => X.Send(It.IsAny<GetStudentsByFiltersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.StudentsByFilterAsync("", 1, [""], [1], [1], [1], pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<StudentsByFiltersResponse>>(okResult.Value);
        Assert.Equal(expectedResult, value);

        _mockMediator.Verify(X => X.Send(
            It.IsAny<GetStudentsByFiltersQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StudentsByFilterAsync_Should_Return_NotFound_WhenResponseIsNullAsync()
    {
        // Arrange
        _mockMediator
            .Setup(X => X.Send(It.IsAny<StudentsByFiltersResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagedResult<StudentsByFiltersResponse>?)null);
        // Act
        var result = await _controller.StudentsByFilterAsync("", 1, [""], [1], [1], [1], new Pagination());
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task StudentsByEvaluationIdAsync_Should_Return_SuccessAsync()
    {
        // Arrange
        var expectedResult = new List<StudentsByFiltersResponse>{
            new StudentsByFiltersResponse() {
                Id = 2,
                Name = "",
                Email = "",
                AnswerId = 2,
                AnswerText = "",
                RiskLevel = 2
            }
        };
        _mockMediator
            .Setup(X => X.Send(It.IsAny<GetStudentsByEvaluationIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.StudentsByEvaluationIdAsync(1, [""], [1], [1], [1]);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);

        _mockMediator.Verify(X => X.Send(
            It.IsAny<GetStudentsByEvaluationIdQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StudentsByEvaluationIdAsync_Should_Return_NotFound_WhenResponseIsNullAsync()
    {
        // Arrange
        _mockMediator
            .Setup(X => X.Send(It.IsAny<GetStudentsByEvaluationIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<List<StudentsByFiltersResponse>>());
        // Act
        var result = await _controller.StudentsByEvaluationIdAsync(
            1,
            new List<string> { "" },
            new List<int> { 1 },
            new List<int> { 10 },
            new List<decimal> { 0.25m });
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
