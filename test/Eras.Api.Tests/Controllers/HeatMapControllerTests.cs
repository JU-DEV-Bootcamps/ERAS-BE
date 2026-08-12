using Eras.Api.Controllers;
using Eras.Application.DTOs.HeatMap;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;
using Eras.Domain.Entities;

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

    [Fact]
    public async Task GetHeatMapSummaryAsync_ReturnsOk_WhenSuccessAsync()
    {
        string pollUUID = "example-correct-uuid";
        var fakeResponse = new GetQueryResponse<HeatMapSummaryResponseVm>(
            null!, "Success", true
        );

        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetHeatMapSummaryAsync(pollUUID);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<BaseResponse>(okResult.Value);
        Assert.IsType<GetQueryResponse<HeatMapSummaryResponseVm>>(okResult.Value);
    }

    [Fact]
    public async Task GetHeatMapSummaryAsync_ReturnsBadRequest_WhenFailureAsync()
    {
        var pollUUID = "example-invalid-uuid";
        var fakeResponse = new GetQueryResponse<HeatMapSummaryResponseVm>(
            null!, "Invalid request", false
        );

        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetHeatMapSummaryAsync(pollUUID);

        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsAssignableFrom<BaseResponse>(badRequestResult.Value);
        Assert.IsType<GetQueryResponse<HeatMapSummaryResponseVm>>(badRequestResult.Value);
    }

    [Fact]
    public async Task GetHeatMapSummaryByFilters_ReturnsOk_WhenSuccessAsync()
    {
        var fakeResponse = new GetQueryResponse<HeatMapSummaryResponseVm>(
            null!, "Success", true
        );

        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapSummaryByFiltersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetHeatMapSummaryByFiltersAsync(2, 5);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<BaseResponse>(okResult.Value);
        Assert.IsType<GetQueryResponse<HeatMapSummaryResponseVm>>(okResult.Value);
    }

    [Fact]
    public async Task GetHeatMapSummaryByFilters_ReturnsBadRequest_WhenFailureAsync()
    {
        var fakeResponse = new GetQueryResponse<HeatMapSummaryResponseVm>(
            null!, "Invalid request", false
        );

        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapSummaryByFiltersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetHeatMapSummaryByFiltersAsync(1, 2);

        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsAssignableFrom<BaseResponse>(badRequestResult.Value);
        Assert.IsType<GetQueryResponse<HeatMapSummaryResponseVm>>(badRequestResult.Value);
    }

    [Fact]
    public async Task GetStudentHeatMapDetailsByComponent_ReturnsOk_WhenSuccessAsync()
    {
        var fakeResponse = new List<StudentHeatMapDetailDto>
        {
            new StudentHeatMapDetailDto
            {
                 StudentId = 1,
                StudentName = "",
                RiskLevel = 2,
                ComponentName = "Test",
            }
        };
        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapDetailsByComponentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetStudentHeatMapDetailsByComponentAsync("Component", 5);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<StudentHeatMapDetailDto>>(okResult.Value);
    }

    [Fact]
    public async Task GetStudentHeatMapDetailsByCohort_ReturnsOk_WhenSuccessAsync()
    {
        var fakeResponse = new List<StudentHeatMapDetailDto>
        {
            new StudentHeatMapDetailDto
            {
                 StudentId = 1,
                StudentName = "",
                RiskLevel = 2,
                ComponentName = "Test",
            }
        };
        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapDetailsByCohortQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);

        IActionResult result = await _controller.GetStudentHeatMapDetailsByCohortAsync("Cohort", 5);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<StudentHeatMapDetailDto>>(okResult.Value);
    }

    [Fact]
    public async Task GetHeatMapDataByPollUuidAndVariableIdsAsync_ReturnsOk_WhenSuccessAsync()
    {
        var fakeResponse = new List<HeatMapBaseData>
        {
            new HeatMapBaseData
            {
                Name = "",
                Data = new List<Serie>{},
            }
        };
        _mediatorMock
            .Setup(Med => Med.Send(It.IsAny<GetHeatMapByPollIdAndVariableIdsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeResponse);
        var request = new HeatMapBaseDataRequestDto
        {
            pollInstanceUuid = "1",
            VariablesIds = new List<int> { 1, 2, 3 }
        };
        IActionResult result = await _controller.GetHeatMapDataByPollUuidAndVariableIdsAsync(request);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<HeatMapBaseData>>(okResult.Value);
    }
}
