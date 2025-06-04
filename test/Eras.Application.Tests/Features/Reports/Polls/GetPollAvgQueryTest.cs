using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Models.Consolidator;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Reports.Polls;

public class GetPollAvgQueryTest
{
    private readonly Mock<IPollInstanceRepository> _mockPollInstanceRepo;
    private readonly Mock<ILogger<PollAvgHandler>> _mockLogger;
    private readonly PollAvgHandler _handler;

    public GetPollAvgQueryTest()
    {
        _mockPollInstanceRepo = new Mock<IPollInstanceRepository>();
        _mockLogger = new Mock<ILogger<PollAvgHandler>>();
        _handler = new PollAvgHandler(_mockLogger.Object, _mockPollInstanceRepo.Object);
    }
    [Fact]
    public async Task Handle_Should_Return_Empty_Response_When_No_PollInstancesAsync()
    {
        // Arrange
        var query = new PollAvgQuery()
        {
            PollUuid = Guid.NewGuid(),
            CohortIds = [1,2]
        };

        // Mock the repository to return an empty list of Answers and map to AvgReportResponseVm
        _mockPollInstanceRepo
            .Setup(Repo => Repo.GetReportByPollCohortAsync(query.PollUuid.ToString(), query.CohortIds))
            .Returns(Task.FromResult(new AvgReportResponseVm())); // Simulate no data

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Body.Components); // Ensure the response is empty
    }
}
