using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Queries;
public class GetPollInstancesByLastDaysQueryHandlerTest
{
    private readonly Mock<IPollInstanceRepository> _mockPollInstanceRepository;
    private readonly Mock<ILogger<GetPollInstancesByLastDaysQueryHandler>> _mockLogger;
    private readonly GetPollInstancesByLastDaysQueryHandler _handler;

    public GetPollInstancesByLastDaysQueryHandlerTest()
    {
        _mockPollInstanceRepository = new Mock<IPollInstanceRepository>();
        _mockLogger = new Mock<ILogger<GetPollInstancesByLastDaysQueryHandler>>();
        _handler = new GetPollInstancesByLastDaysQueryHandler(_mockPollInstanceRepository.Object, _mockLogger.Object);
    }

    private static PollInstance BuildPollInstance(int Id = 1, string Uuid = "m0ck-Uu1D")
        => new ()
        {
            Id = Id,
            Uuid = Uuid,
            LastVersion = 1,
            LastVersionDate = DateTime.Now,
            FinishedAt = DateTime.Now
        };
    
    [Fact]
    public async Task Handler_ShouldReturnSuccessResponse()
    {
        var pollInstances = new List<PollInstance>
        {
            BuildPollInstance(),
            BuildPollInstance(2, "m0ck-2")
        };
        var query = new GetPollInstancesByLastDaysQuery { LastDays = 1, LastVersion = true, PollUuid = "m0ck-Uu1D" };

        _mockPollInstanceRepository.Setup(Repo => Repo.GetByLastDays(1, true, "m0ck-Uu1D"))
            .ReturnsAsync(pollInstances);

        GetQueryResponse<List<PollInstance>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result.Body);
        Assert.True(result.Success);
        Assert.Equal("PollInstances obtained", result.Message);
        Assert.Collection(result.Body,
            item => Assert.Equal(pollInstances[0], item),
            item => Assert.Equal(pollInstances[1], item)
        );
    }

    [Fact]
    public async Task Handler_ShouldReturnFailureResponse_WhenCatchingException()
    {
        var query = new GetPollInstancesByLastDaysQuery { LastDays = 1, LastVersion = true, PollUuid = "m0ck-Uu1D" };

        _mockPollInstanceRepository.Setup(Repo => Repo.GetByLastDays(1, true, "m0ck-Uu1D"))
            .ThrowsAsync(new Exception("DB Error."));

        GetQueryResponse<List<PollInstance>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result.Body);
        Assert.Empty(result.Body);
        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }
}