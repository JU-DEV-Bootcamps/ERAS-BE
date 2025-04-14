using Eras.Api.Controllers;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Models;
using Eras.Application.Models.HeatMap;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class HeatMapControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<HeatMapController>> _loggerMock;
    private readonly HeatMapController _controller;

    public HeatMapControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<HeatMapController>>();
        _controller = new HeatMapController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetHeatMapDataByAllComponents_ReturnsOk_WhenSuccessAsync()
    {
        string pollUUID = "example-correct-uuid";
        var fakeResponse = new GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>(
            [], "Success", true
        );

        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapDataByAllComponentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetHeatMapDataByAllComponentsAsync(pollUUID);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<BaseResponse>(okResult.Value);
        Assert.IsType<GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>>(okResult.Value);
    }

    [Fact]
    public async Task GetHeatMapDataByAllComponents_ReturnsBadRequest_WhenFailureAsync()
    {
        var pollUUID = "example-invalid-uuid";
        var fakeResponse = new GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>(
            [], "Invalid request", false
        );

        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapDataByAllComponentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetHeatMapDataByAllComponentsAsync(pollUUID);

        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsAssignableFrom<BaseResponse>(badRequestResult.Value);
        Assert.IsType<GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>>(badRequestResult.Value);
    }
}
