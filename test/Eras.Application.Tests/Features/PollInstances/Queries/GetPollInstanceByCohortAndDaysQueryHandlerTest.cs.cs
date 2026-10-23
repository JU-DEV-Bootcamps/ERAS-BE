using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Eras.Application.Utils;
using Moq;
using Eras.Application.Models.Response.Common;
using Eras.Application.DTOs;

namespace Eras.Application.Tests.Features.PollInstances.Queries;
public class GetPollInstanceByCohortAndDaysQueryHandlerTest
{
    private readonly Mock<IPollInstanceRepository> _mockPollInstanceRepository;
    private readonly Mock<IEvaluationRepository> _mockEvaluationRepository; 
    private readonly Mock<ILogger<GetPollInstanceByCohortAndDaysQueryHandler>> _mockLogger;
    private readonly GetPollInstanceByCohortAndDaysQueryHandler _handler;

    public GetPollInstanceByCohortAndDaysQueryHandlerTest()
    {
        _mockPollInstanceRepository = new Mock<IPollInstanceRepository>();
        _mockEvaluationRepository = new Mock<IEvaluationRepository>();
        _mockLogger = new Mock<ILogger<GetPollInstanceByCohortAndDaysQueryHandler>>();
        _handler = new GetPollInstanceByCohortAndDaysQueryHandler(_mockPollInstanceRepository.Object, _mockEvaluationRepository.Object, _mockLogger.Object);
    }

    private static PollInstance BuildPollInstance(DateTime? FinishedAt, int Id = 1, string Uuid = "m0ck-Uu1D")
        => new ()
        {
            Id = Id,
            Uuid = Uuid,
            LastVersion = 1,
            LastVersionDate = DateTime.Now,
            FinishedAt = FinishedAt ?? DateTime.Now
        };

    [Fact]
    public async Task Handle_ShouldReturnSuccessResponse()
    {
        var cohortId = new int[] { 1, 2 };
        var days = 10;
        var pagination = new Pagination();
        var pollUuid = "poll-uuid";


        var query = new GetPollInstanceByCohortAndDaysQuery(pagination, cohortId, days, true, pollUuid);
        var pollInstances = new List<PollInstance>
        {
            BuildPollInstance(DateTime.Now),
            BuildPollInstance(DateTime.UtcNow.AddDays(-5), 2, "mock-Uu2D")
        };
        var pagedResult = new PagedResult<PollInstance>(pollInstances.Count, pollInstances);

        _mockPollInstanceRepository
            .Setup(Repo => Repo.GetByCohortIdAndLastDays(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int[]>(),
                It.IsAny<int?>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int?>())
            )
            .ReturnsAsync(pagedResult);

        GetQueryResponse<PagedResult<PollInstanceDTO>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Body);
        Assert.Equal(2, result.Body.Count);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithStartDateAndEndDateIfEvaluationIsFound()
    {
        var cohortId = new int[] { 1, 2 };
        var days = 10;
        var pagination = new Pagination();
        var pollUuid = "poll-uuid";
        var evaluationId = 1;

        var pollInstances = new List<PollInstance>
        {
            BuildPollInstance(DateTime.Now),
            BuildPollInstance(DateTime.UtcNow.AddDays(-5), 2, "mock-Uu2D")
        };
        var evaluation = new Evaluation
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        var query = new GetPollInstanceByCohortAndDaysQuery(pagination, cohortId, days, true, pollUuid, evaluationId);

        _mockEvaluationRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(evaluation);

        GetQueryResponse<PagedResult<PollInstanceDTO>> result = await _handler.Handle(query, CancellationToken.None);

        _mockPollInstanceRepository.Verify(Repo => Repo.GetByCohortIdAndLastDays(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int[]>(),
                It.IsAny<int?>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int?>()), Times.Once);
    }

    [Fact]
    public async Task Handler_ShouldThrowExceptionIfEvaluationNotFound()
    {
        var cohortId = new int[] { 1, 2 };
        var days = 10;
        var pagination = new Pagination();
        var pollUuid = "poll-uuid";
        var evaluationId = 1;

        var query = new GetPollInstanceByCohortAndDaysQuery(pagination, cohortId, days, true, pollUuid, evaluationId);

        _mockEvaluationRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(value: null);

        GetQueryResponse<PagedResult<PollInstanceDTO>> result = await _handler.Handle(query, CancellationToken.None);

        _mockEvaluationRepository.Verify(Repo => Repo.GetByIdAsync(It.IsAny<int>()), Times.Once);
        Assert.False(result.Success);
        Assert.Equal("Failed", result.Message);
        Assert.Empty(result.Body!.Items);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResponseOnException()
    {
        var cohortId = new int[] { 1, 2 };
        var days = 10;
        var pagination = new Pagination();

        var query = new GetPollInstanceByCohortAndDaysQuery(pagination, cohortId, days, true, "poll-Uuid");

        _mockPollInstanceRepository
            .Setup(Repo => Repo.GetByCohortIdAndLastDays(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int[]>(),
                It.IsAny<int?>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int?>())
            )
            .ThrowsAsync(new Exception("Database error"));

        GetQueryResponse<PagedResult<PollInstanceDTO>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Failed", result.Message);
        Assert.Empty(result.Body!.Items);
    }
}
