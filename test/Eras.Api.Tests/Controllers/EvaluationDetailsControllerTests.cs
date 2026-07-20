using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Api.Controllers;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsRecentAlerts;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
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
        var expectedResult = new PagedResult<GetStudentsRecentAlertsResponse>(2, new List<GetStudentsRecentAlertsResponse>{ new (), new () }); 
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
}
