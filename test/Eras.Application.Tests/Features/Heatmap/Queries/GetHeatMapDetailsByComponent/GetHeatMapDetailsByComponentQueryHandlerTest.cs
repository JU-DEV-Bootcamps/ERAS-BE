using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Heatmap.Queries.GetHeatMapDetailsByComponent;

public class GetHeatMapDetailsByComponentQueryHandlerTest
{
    private readonly Mock<IStudentRepository> _studentRepository;
    private readonly Mock<ILogger<GetHeatMapDetailsByComponentQueryHandler>> _logger;
    private readonly GetHeatMapDetailsByComponentQueryHandler _handler;

    public GetHeatMapDetailsByComponentQueryHandlerTest()
    {
        _studentRepository = new Mock<IStudentRepository>();
        _logger = new Mock<ILogger<GetHeatMapDetailsByComponentQueryHandler>>();
        _handler = new GetHeatMapDetailsByComponentQueryHandler(_studentRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldGetHeatMapDetailsByComponent_Successfully()
    {
        var heatmapDetailsList = new List<StudentHeatMapDetailDto>
        {
            new StudentHeatMapDetailDto
            {
                StudentName = "Joe",
                ComponentName = "Act"
            },
            new StudentHeatMapDetailDto
            {
                StudentName = "Dan",
                ComponentName = "Act"
            },
        };

        var request = new GetHeatMapDetailsByComponentQuery("Act", 2);

        _studentRepository
            .Setup(x => x.GetStudentHeatMapDetailsByComponent("Act", 2))
            .ReturnsAsync(heatmapDetailsList);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
