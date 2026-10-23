using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Cohorts.Queries;
using Eras.Application.Models.Response.Controllers.CohortsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Test.Features.Cohorts.Queries;
public class GetCohortsSummaryQueryHandlerTest
{
    private readonly Mock<IStudentCohortRepository> _mockSCRepository;
    private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
    private readonly Mock<ILogger<GetCohortsSummaryQuery>> _mockLogger;
    private readonly GetCohortsSummaryQueryHandler _handler;

    public GetCohortsSummaryQueryHandlerTest()
    {
        _mockSCRepository = new Mock<IStudentCohortRepository>();
        _mockEvaluationRepository = new Mock<IEvaluationRepository>();
        _mockLogger = new Mock<ILogger<GetCohortsSummaryQuery>>();
        _handler = new GetCohortsSummaryQueryHandler(_mockSCRepository.Object, _mockEvaluationRepository.Object, _mockLogger.Object);
    }

    private static StudentSummary BuildStudentSummary(
        string StudentUuid = "mock-uuid",
        string StudentName = "Marcela Lopez",
        int CohortId = 1,
        string CohortName = "Cohort_2026",
        decimal PollinstancesAverage = 3,
        int PollinstancesCount = 1
    )
    {
        return new StudentSummary
        {
            StudentUuid = StudentUuid,
            StudentName = StudentName,
            CohortId = CohortId,
            CohortName = CohortName,
            PollinstancesAverage = PollinstancesAverage,
            PollinstancesCount = PollinstancesCount
        };
    }

    [Fact]
    public async Task Handler_ShouldCallRepoMethodWithNullDatesIfEvaluationIdNotProvided()
    {
        var cohortSummary = new CohortSummaryResponse
        {
            CohortCount = 1,
            StudentCount = 2,
            Summary = new List<StudentSummary>
            {
                BuildStudentSummary(),
                BuildStudentSummary("new-mock-uuid", "Carlos Calipso")
            }
        };
        var pagination = new Pagination { PageSize = 10 };

        _mockSCRepository.Setup(Repo => Repo.GetCohortsSummaryAsync(pagination, null, null))
            .ReturnsAsync(cohortSummary);

        var query = new GetCohortsSummaryQuery { Pagination = pagination };

        CohortSummaryResponse result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        _mockSCRepository.Verify(Repo => Repo.GetCohortsSummaryAsync(It.IsAny<Pagination>(), null, null), Times.Once);
        _mockEvaluationRepository.Verify(Repo => Repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldReturnCohortSummaryUsingEvaluationDates()
    {
        var cohortSummary = new CohortSummaryResponse
        {
            CohortCount = 1,
            StudentCount = 2,
            Summary = new List<StudentSummary>
            {
                BuildStudentSummary(),
                BuildStudentSummary("new-mock-uuid", "Carlos Calipso")
            }
        };

        var evaluation = new Evaluation
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        var pagination = new Pagination { PageSize = 10 };

        _mockSCRepository.Setup(Repo => Repo.GetCohortsSummaryAsync(
            pagination,
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>()
        )).ReturnsAsync(cohortSummary);
        _mockEvaluationRepository.Setup(Repo => Repo.GetByIdAsync(1))
            .ReturnsAsync(evaluation);

        var query = new GetCohortsSummaryQuery
        {
            Pagination = pagination,
            EvaluationId = 1
        };

        CohortSummaryResponse result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        _mockEvaluationRepository.Verify(Repo => Repo.GetByIdAsync(1), Times.Once);
        _mockSCRepository.Verify(
            Repo => Repo.GetCohortsSummaryAsync(It.IsAny<Pagination>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handler_ShouldThrowExceptionIfEvaluationDoesNotExist()
    {
        _mockEvaluationRepository.Setup(Repo => Repo.GetByIdAsync(1)).ReturnsAsync(value: null);

        var query = new GetCohortsSummaryQuery
        {
            Pagination = new Pagination(),
            EvaluationId = 1
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _handler.Handle(query, CancellationToken.None)
        );

        _mockEvaluationRepository.Verify(Repo => Repo.GetByIdAsync(1), Times.Once);
        _mockSCRepository.Verify(
            Repo => Repo.GetCohortsSummaryAsync(It.IsAny<Pagination>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Never
        );        
    }
}