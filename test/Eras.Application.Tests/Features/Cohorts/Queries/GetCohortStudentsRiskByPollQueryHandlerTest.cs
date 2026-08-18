using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Cohorts.Queries.GetCohortStudentsRiskByPoll;
using Eras.Application.Models.Response.Calculations;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Cohorts.Queries;
public class GetCohortStudentsRiskByPollQueryHandlerTest
{
    private readonly Mock<IPollCohortRepository> _mockPollCohortRepository;
    private readonly Mock<ILogger<GetCohortStudentsRiskByPollQueryHandler>> _mockLogger;
    private readonly GetCohortStudentsRiskByPollQueryHandler _handler;

    public GetCohortStudentsRiskByPollQueryHandlerTest()
    {
        _mockPollCohortRepository = new Mock<IPollCohortRepository>();
        _mockLogger = new Mock<ILogger<GetCohortStudentsRiskByPollQueryHandler>>();
        _handler = new GetCohortStudentsRiskByPollQueryHandler(_mockPollCohortRepository.Object, _mockLogger.Object);
    }

    private static GetCohortStudentsRiskByPollResponse BuildCohortComponentByPoll(int PollInstanceId = 1, string StudentName = "Juan Perez", decimal PollInstanceRiskSum = 3)
    {
        return new GetCohortStudentsRiskByPollResponse
        {
            PollInstanceId = PollInstanceId,
            StudentName = StudentName,
            PollInstanceRiskSum = PollInstanceRiskSum
        };
    }
    
    [Fact]
    public async Task Handle_ShouldReturnListOfCohortComponentsByPoll()
    {
        var cohorts = new List<GetCohortStudentsRiskByPollResponse>
        {
            BuildCohortComponentByPoll(),
            BuildCohortComponentByPoll(PollInstanceId:2, StudentName:"Maria Montes")
        };

        _mockPollCohortRepository.Setup(Repo => Repo.GetCohortStudentsRiskByPoll("mock_uuid", 1))
            .ReturnsAsync(cohorts);

        var query = new GetCohortStudentsRiskByPollQuery { PollUuid = "mock_uuid", CohortId = 1 };

        List<GetCohortStudentsRiskByPollResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList()
    {
        _mockPollCohortRepository.Setup(Repo => Repo.GetCohortStudentsRiskByPoll("mock_uuid", 1))
            .ReturnsAsync([]);

        var query = new GetCohortStudentsRiskByPollQuery { PollUuid = "mock_uuid", CohortId = 1 };

        List<GetCohortStudentsRiskByPollResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}