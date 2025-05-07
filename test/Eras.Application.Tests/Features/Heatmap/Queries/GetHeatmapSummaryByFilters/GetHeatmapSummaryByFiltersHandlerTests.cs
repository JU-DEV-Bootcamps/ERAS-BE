
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Heatmap.Queries.GetHeatmapSummaryByFilters;

public class GetHeatMapSummaryByFiltersHandlerTests
{
    private readonly Mock<IHeatMapRepository> _mockHeatMapRepository;
    private readonly Mock<ILogger<GetHeatMapSummaryByFiltersHandler>> _mockLogger;
    private readonly GetHeatMapSummaryByFiltersHandler _handler;

    public GetHeatMapSummaryByFiltersHandlerTests()
    {
        _mockHeatMapRepository = new Mock<IHeatMapRepository>();
        _mockLogger = new Mock<ILogger<GetHeatMapSummaryByFiltersHandler>>();
        _handler = new GetHeatMapSummaryByFiltersHandler(_mockHeatMapRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResponse_WhenDataIsFoundAsync()
    {

        var query = new GetHeatMapSummaryByFiltersQuery(1, 7);
        var heatMapData = new List<GetHeatMapByComponentsQueryResponse>
        {
            new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableName = "Variable1", AnswerText = "Answer 1" }
        };
        _mockHeatMapRepository.Setup(Repo => Repo.GetHeatMapDataByCohortAndDaysAsync(1, 7))
            .ReturnsAsync(heatMapData);

        GetQueryResponse<HeatMapSummaryResponseVm> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Body);
        Assert.NotEmpty(result.Body.Components);
        _mockHeatMapRepository.Verify(Repo => Repo.GetHeatMapDataByCohortAndDaysAsync(1, 7), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResponse_WhenNoDataIsFoundAsync()
    {
        var query = new GetHeatMapSummaryByFiltersQuery(1, 7);
        _mockHeatMapRepository.Setup(Repo => Repo.GetHeatMapDataByCohortAndDaysAsync(1, 7))
            .ReturnsAsync([]);

        GetQueryResponse<HeatMapSummaryResponseVm> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Body);
        Assert.Empty(result.Body.Components);
        _mockHeatMapRepository.Verify(Repo => Repo.GetHeatMapDataByCohortAndDaysAsync(1, 7), Times.Once);
    }
}
