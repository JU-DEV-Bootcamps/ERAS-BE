using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Cohorts.Queries;
public class GetCohortTopRiskStudentsQueryHandlerTest
{
    private readonly Mock<ICohortRepository> _mockCohortRepository;
    private readonly Mock<ILogger<GetCohortTopRiskStudentsQueryHandler>> _mockLogger;
    private readonly GetCohortTopRiskStudentsQueryHandler _handler;
    public GetCohortTopRiskStudentsQueryHandlerTest()
    {
        _mockCohortRepository = new Mock<ICohortRepository>();
        _mockLogger = new Mock<ILogger<GetCohortTopRiskStudentsQueryHandler>>();
        _handler = new GetCohortTopRiskStudentsQueryHandler(_mockCohortRepository.Object, _mockLogger.Object);
    }

    private static GetCohortTopRiskStudentsByComponentResponse BuildCohortTopRiskStudents(
        int StudentId = 1, string StudentName = "Juan Perez", decimal AnswerAverage = 3, decimal RiskSum = 4
    )
    {
        return new GetCohortTopRiskStudentsByComponentResponse
        {
            StudentId = StudentId,
            StudentName = StudentName,
            AnswerAverage = AnswerAverage,
            RiskSum = RiskSum
        };
    }
    
    [Fact]
    public async Task Handler_ShouldReturnPagedResultWithCohortTopRiskStudents()
    {
        var cohorts = new List<GetCohortTopRiskStudentsByComponentResponse>
        {
            BuildCohortTopRiskStudents(),
            BuildCohortTopRiskStudents(2, "Maria Montes")
        };

        _mockCohortRepository.Setup(Repo => Repo.GetCohortTopRiskStudentsAsync("mock_uuid", 1, true, 1, 10))
            .ReturnsAsync(cohorts);
        _mockCohortRepository.Setup(Repo => Repo.CountStudentsAsync("mock_uuid", 1, true, null))
            .ReturnsAsync(2);

        var query = new GetCohortTopRiskStudentsQuery {
            PollUuid = "mock_uuid",
            CohortId = 1,
            LastVersion = true,
            PageValues = new Pagination
            {
                Page = 0,
                PageSize = 10
            }
        };

        GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result.Body);
        Assert.IsType<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(result.Body);
        Assert.Equal(2, result.Body.Count);
        Assert.Equal(2, result.Body.Items.Count);
    }

    [Fact]
    public async Task Handler_ShouldHandleExceptionAndReturnPagedResultWithEmptyList()
    {
        _mockCohortRepository.Setup(Repo => Repo.GetCohortTopRiskStudentsAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>())
        ).ThrowsAsync(new Exception("Error retrieving cohorts."));

        var query = new GetCohortTopRiskStudentsQuery {
            PollUuid = "mock_uuid",
            CohortId = 1,
            LastVersion = true,
            PageValues = new Pagination
            {
                Page = 0,
                PageSize = 10
            }
        };

        GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>> result = await _handler.Handle(query, CancellationToken.None);

        _mockCohortRepository.Verify(
            Repo => Repo.CountStudentsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()),
            Times.Never
        );
        Assert.NotNull(result.Body);
        Assert.IsType<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(result.Body);
        Assert.Equal(0, result.Body.Count);
        Assert.Empty(result.Body.Items);
    }

    [Fact]
    public async Task Handler_ShouldReturnPagedResultWithEmptyList()
    {
        _mockCohortRepository.Setup(Repo => Repo.GetCohortTopRiskStudentsAsync("mock_uuid", 1, true, 1, 10))
            .ReturnsAsync([]);
        _mockCohortRepository.Setup(Repo => Repo.CountStudentsAsync("mock_uuid", 1, true, "Test"))
            .ReturnsAsync(0);

        var query = new GetCohortTopRiskStudentsQuery {
            PollUuid = "mock_uuid",
            CohortId = 1,
            LastVersion = true,
            PageValues = new Pagination
            {
                Page = 0,
                PageSize = 10
            }
        };

        GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result.Body);
        Assert.IsType<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(result.Body);
        Assert.Equal(0, result.Body.Count);
        Assert.Empty(result.Body.Items);
    }
}