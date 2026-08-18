using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Cohorts.Queries;
using Eras.Application.Features.Cohorts.Queries.GetCohortsList;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Cohorts.Queries;
public class GetCohortsListQueryHandlerTest
{
    private readonly Mock<ICohortRepository> _mockRepository;
    private readonly Mock<ILogger<GetCohortsListQueryHandler>> _mockLogger;
    private readonly GetCohortsListQueryHandler _handler;

    public GetCohortsListQueryHandlerTest()
    {
        _mockRepository = new Mock<ICohortRepository>();
        _mockLogger = new Mock<ILogger<GetCohortsListQueryHandler>>();
        _handler = new GetCohortsListQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    private static Cohort BuildCohort(string Name = "Test_Cohort", string CourseCode = "Test_Course")
    {
        return new Cohort
        {
            Name = Name,
            CourseCode = CourseCode
        };
    }

    [Fact]
    public async Task Handler_ShouldReturnAllCohortsIfNoPollUuidIsProvided()
    {
        var cohorts = new List<Cohort>
        {
            BuildCohort(),
            BuildCohort("Cohort_2026_A", "Course-001"),
            BuildCohort("Cohort_2026_B", "Course-002")
        };
        var returnMessage = $"All {cohorts.Count} Cohorts retrieved successfully";

        _mockRepository.Setup(Repo => Repo.GetCohortsAsync()).ReturnsAsync(cohorts);

        var query = new GetCohortsListQuery();

        GetQueryResponse<List<Cohort>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Body);
        Assert.Equal(3, result.Body.Count);
        Assert.Equal(returnMessage, result.Message);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handler_ShouldReturnCohortsByPollUuid()
    {
        var cohorts = new List<Cohort>
        {
            BuildCohort(),
            BuildCohort("Cohort_2026_A", "Course-001")
        };
        var pollUuid = "mock-uuid";
        var returnMessage = $"{cohorts} cohorts retrieved from poll {pollUuid} successfully";

        _mockRepository.Setup(Repo => Repo.GetCohortsByPollUuidAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(cohorts);

        var query = new GetCohortsListQuery { PollUuid = pollUuid };

        GetQueryResponse<List<Cohort>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Body);
        Assert.Equal(2, result.Body.Count);
        Assert.Equal(returnMessage, result.Message);
        Assert.True(result.Success);
    }
}