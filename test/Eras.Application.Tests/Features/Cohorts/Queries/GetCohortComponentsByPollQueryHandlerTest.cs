using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Cohorts.Queries.GetCohortComponentsByPoll;
using Eras.Application.Models.Response.Calculations;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Cohorts.Queries;
public class GetCohortComponentsByPollQueryHandlerTest
{
    private readonly Mock<IPollCohortRepository> _mockPollCohortRepository;
    private readonly Mock<ILogger<GetCohortComponentsByPollQueryHandler>> _mockLogger;
    private readonly GetCohortComponentsByPollQueryHandler _handler;
    public GetCohortComponentsByPollQueryHandlerTest()
    {
        _mockPollCohortRepository = new Mock<IPollCohortRepository>();
        _mockLogger = new Mock<ILogger<GetCohortComponentsByPollQueryHandler>>();
        _handler = new GetCohortComponentsByPollQueryHandler(_mockPollCohortRepository.Object, _mockLogger.Object);
    }
    
    private static GetCohortComponentsByPollResponse BuildCohortComponentByPoll(int CohortId = 1, string CohortName = "Test", string ComponentName = "Test")
    {
        return new GetCohortComponentsByPollResponse
        {
            CohortId = CohortId,
            CohortName = CohortName,
            ComponentName = ComponentName
        };
    }
    
    [Fact]
    public async Task Handle_ShouldReturnListOfCohortComponentsByPoll()
    {
        var components = new List<GetCohortComponentsByPollResponse>
        {
            BuildCohortComponentByPoll(),
            BuildCohortComponentByPoll(CohortId:2)
        };

        _mockPollCohortRepository.Setup(Repo => Repo.GetCohortComponentsByPoll("mock_uuid", true))
            .ReturnsAsync(components);

        var query = new GetCohortComponentsByPollQuery { PollUuid = "mock_uuid", LastVersion = true };

        List<GetCohortComponentsByPollResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList()
    {
        _mockPollCohortRepository.Setup(Repo => Repo.GetCohortComponentsByPoll("mock_uuid", true))
            .ReturnsAsync([]);

        var query = new GetCohortComponentsByPollQuery { PollUuid = "mock_uuid", LastVersion = true };

        List<GetCohortComponentsByPollResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}