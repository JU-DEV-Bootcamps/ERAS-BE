using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Polls.Queries;
public class GetPollsByCohortListQueryHandlerTest
{
    private readonly Mock<IPollCohortRepository> _mockRepository;
    private readonly Mock<ILogger<GetPollsByCohortListQueryHandler>> _mockLogger;
    private readonly GetPollsByCohortListQueryHandler _handler;

    public GetPollsByCohortListQueryHandlerTest()
    {
        _mockRepository = new Mock<IPollCohortRepository>();
        _mockLogger = new Mock<ILogger<GetPollsByCohortListQueryHandler>>();
        _handler = new GetPollsByCohortListQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    private static Poll BuildPoll(
        int Id = 1,
        string Name = "Test poll",
        string Uuid = "m0ck-Uu1D",
        string ParentId = "id-01") => new()
        {
            Id = Id,
            Name = Name,
            Uuid = Uuid,
            Audit = new Domain.Common.AuditInfo(),
            LastVersion = 1,
            LastVersionDate = DateTime.Now,
            ParentId = ParentId
        };

    [Fact]
    public async Task Handler_ShouldReturnListOfGetPollsQueryResponse()
    {
        var polls = new List<Poll>
        {
            BuildPoll(),
            BuildPoll(2, "Second Poll", "mock-UuiD", "id-02")
        };

        var getPollsQueryResponses = polls.Select(Poll => new GetPollsQueryResponse
        {
            Id = Poll.Id,
            Uuid = Poll.Uuid,
            Name = Poll.Name,
            LastVersion = Poll.LastVersion,
            LastVersionDate = Poll.LastVersionDate,
        }).ToList();

        var query = new GetPollsByCohortListQuery { CohortId = 1 };

        _mockRepository.Setup(Repo => Repo.GetPollsByCohortIdAsync(1))
            .ReturnsAsync(polls);

        List<GetPollsQueryResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(getPollsQueryResponses.Count, result.Count);
        Assert.Collection(result,
            item => Assert.Equivalent(getPollsQueryResponses[0], result[0]),
            item => Assert.Equivalent(getPollsQueryResponses[1], result[1])
        );
    }
}