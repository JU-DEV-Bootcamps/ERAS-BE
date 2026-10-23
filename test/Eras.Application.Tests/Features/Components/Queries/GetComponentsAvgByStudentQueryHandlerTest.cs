using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Components.Queries;
public class GetComponentsAvgByStudentQueryHandlerTest
{
    private readonly Mock<IComponentsAvgRepository> _mockComponentsAvgRepository;
    private readonly Mock<ILogger<GetComponentsAvgByStudentQueryHandler>> _mockLogger;
    private readonly GetComponentsAvgByStudentQueryHandler _handler;

    public GetComponentsAvgByStudentQueryHandlerTest()
    {
        _mockComponentsAvgRepository = new Mock<IComponentsAvgRepository>();
        _mockLogger = new Mock<ILogger<GetComponentsAvgByStudentQueryHandler>>();
        _handler = new GetComponentsAvgByStudentQueryHandler(_mockComponentsAvgRepository.Object, _mockLogger.Object);
    }

    private static ComponentsAvg BuildComponentsAvgObject(int PollId = 1, int ComponentId = 1, string Name = "Test", float ComponentAvg = 3)
    {
        return new ComponentsAvg
        {
            PollId = PollId,
            ComponentId = ComponentId,
            Name = Name,
            ComponentAvg = ComponentAvg 
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfComponentsAvg()
    {
        var components = new List<ComponentsAvg>
        {
            BuildComponentsAvgObject(1, 1, "Test", 2),
            BuildComponentsAvgObject(1, 2, "Test2", 4)    
        };

        _mockComponentsAvgRepository.Setup(Repo => Repo.ComponentsAvgByStudent(1, 1))
            .ReturnsAsync(components);

        var query = new GetComponentsAvgByStudentQuery { PollId = 1, StudentId = 1 };

        List<ComponentsAvg> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList()
    {
        _mockComponentsAvgRepository.Setup(Repo => Repo.ComponentsAvgByStudent(1, 1))
            .ReturnsAsync(new List<ComponentsAvg>());

        var query = new GetComponentsAvgByStudentQuery { PollId = 1, StudentId = 1 };

        List<ComponentsAvg> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}