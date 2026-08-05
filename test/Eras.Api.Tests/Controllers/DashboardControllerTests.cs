using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Dashboard.Queries.GetDashboardKpis;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DashboardController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetKpis_Should_Return_Ok_WithSuccessQueryAsync()
    {
        // Arrange
        var dashboardDTO = new DashboardKpiDto() { 
                TotalEvaluations = new KpiMetricDto { PercentageChange = 20, Value = 2 }, 
                TotalPollsAnswered = new KpiMetricDto { PercentageChange = 20, Value = 2 }, 
                TotalStudents = new KpiMetricDto { PercentageChange = 20, Value = 2 }};
        var commandResponse = new GetQueryResponse<DashboardKpiDto>(dashboardDTO, "Success", true);
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<GetDashboardKpisQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);
        // Act
        var result = await _controller.GetKpis();
        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetKpis_Should_Return_BadRequest_When_Query_Fails()
    {
        // Arrange
        var response = new GetQueryResponse<DashboardKpiDto>(
            null,
            "Error retrieving KPIs",
            false);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDashboardKpisQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        // Act
        var result = await _controller.GetKpis();
        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response, badRequest.Value);
    }
}
