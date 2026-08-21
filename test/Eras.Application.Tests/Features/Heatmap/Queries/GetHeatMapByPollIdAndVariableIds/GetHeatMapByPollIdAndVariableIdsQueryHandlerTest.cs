using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds;
using Eras.Application.Models.Response.HeatMap;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Heatmap.Queries.GetHeatMapByPollIdAndVariableIds;

public class GetHeatMapByPollIdAndVariableIdsQueryHandlerTest
{
    private readonly Mock<IHeatMapRepository> _heatmapRepository;
    private readonly Mock<ILogger<GetHeatMapByPollIdAndVariableIdsQueryHandler>> _logger;
    private readonly GetHeatMapByPollIdAndVariableIdsQueryHandler _handler;

    public GetHeatMapByPollIdAndVariableIdsQueryHandlerTest()
    {
        _heatmapRepository = new Mock<IHeatMapRepository>();
        _logger = new Mock<ILogger<GetHeatMapByPollIdAndVariableIdsQueryHandler>>();
        _handler = new GetHeatMapByPollIdAndVariableIdsQueryHandler(_heatmapRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldGetHeatMapByPollIdAndVariableIds_Successfully()
    {
        var heatmapList = new List<HeatMapBaseData>()
        {
            new HeatMapBaseData
            {
                Name = "test",
                Data = new List<Serie>()
            }
        };
        var request = new GetHeatMapByPollIdAndVariableIdsQuery("123", [1]);

        _heatmapRepository
            .Setup(x => x.GetHeatMapByPollUuidAndVariableIds("123", It.IsAny<List<int>>()))
            .ReturnsAsync(heatmapList);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(response);
        Assert.Single(heatmapList);
        Assert.Equal("test", heatmapList[0].Name);
    }
}
