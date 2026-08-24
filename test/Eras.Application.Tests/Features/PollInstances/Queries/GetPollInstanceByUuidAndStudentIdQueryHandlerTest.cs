using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Queries;
public class GetPollInstanceByUuidAndStudentIdQueryHandlerTest
{
    private readonly Mock<IPollInstanceRepository> _mockPollRepository;
    private readonly Mock<ILogger<GetPollInstanceByUuidAndStudentIdQueryHandler>> _mockLogger;
    private readonly GetPollInstanceByUuidAndStudentIdQueryHandler _handler;

    public GetPollInstanceByUuidAndStudentIdQueryHandlerTest()
    {
        _mockPollRepository = new Mock<IPollInstanceRepository>();
        _mockLogger = new Mock<ILogger<GetPollInstanceByUuidAndStudentIdQueryHandler>>();
        _handler = new GetPollInstanceByUuidAndStudentIdQueryHandler(_mockPollRepository.Object, _mockLogger.Object);
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
        PollInstance pollInstance = BuildPollInstance();

        var query = new GetPollInstanceByUuidAndStudentIdQuery() { PollUuid = "uuid1", StudentId = 1, EvaluationId = 1 };

        _mockPollRepository
            .Setup(Repo => Repo.GetByUuidAndStudentIdAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(pollInstance);

        GetQueryResponse<PollInstance> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Poll Found", result.Message);
        Assert.NotNull(result.Body);
        Assert.Equal(pollInstance, result.Body);
    }

    [Fact]
    public async Task Handler_ShouldReturnNotFoundResponseIfPollInstanceNotFound()
    {
        var query = new GetPollInstanceByUuidAndStudentIdQuery { PollUuid = "uuid1", StudentId = 1, EvaluationId = 1 };
        _mockPollRepository
            .Setup(Repo => Repo.GetByUuidAndStudentIdAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(value: null);
        
        GetQueryResponse<PollInstance> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Not Found", result.Message);
        Assert.NotNull(result.Body);
        Assert.Equal(QueryEnums.QueryResultStatus.NotFound, result.Status);
    }
}
