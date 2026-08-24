using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Poll;
using Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Polls.Queries;
public class GetAllByPollAndCohortQueryHandlerTest
{
    private readonly Mock<IPollCohortRepository> _mockPollCohortRepository;
    private readonly Mock<ILogger<GetAllByPollAndCohortQueryHandler>> _mockLogger;
    private readonly GetAllByPollAndCohortQueryHandler _handler;

    public GetAllByPollAndCohortQueryHandlerTest()
    {
        _mockPollCohortRepository = new Mock<IPollCohortRepository>();
        _mockLogger = new Mock<ILogger<GetAllByPollAndCohortQueryHandler>>();
        _handler = new GetAllByPollAndCohortQueryHandler(_mockPollCohortRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handler_ShouldReturnListOfPollVariables()
    {
        var pollVariables = new List<PollVariableDto>
        {
          new () { PollId = 1, VariableId = 1, VariableName = "Academic"},  
          new () { PollId = 1, VariableId = 2, VariableName = "Social"}  
        };
        var query = new GetAllByPollAndCohortQuery(1, 1);

        _mockPollCohortRepository.Setup(Repo => Repo.GetPollVariablesAsync(1, 1))
            .ReturnsAsync(pollVariables);

        List<PollVariableDto> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(pollVariables.Count, result.Count);
        Assert.Collection(result,
            item => Assert.Equal(pollVariables[0], result[0]),
            item => Assert.Equal(pollVariables[1], result[1])
        );
    }
}