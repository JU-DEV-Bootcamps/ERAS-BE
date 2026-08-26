using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohortAndPoll;
using Eras.Application.Utils;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries;
public class GetAllAverageRiskByCohortAndPollQueryHandlerTest
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ILogger<GetAllAverageRiskByCohortAndPollQueryHandler>> _mockLogger;
    private readonly GetAllAverageRiskByCohortAndPollQueryHandler _handler;

    public GetAllAverageRiskByCohortAndPollQueryHandlerTest()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<GetAllAverageRiskByCohortAndPollQueryHandler>>();
        _handler = new GetAllAverageRiskByCohortAndPollQueryHandler(_mockStudentRepository.Object, _mockLogger.Object);
    }

    private static StudentAverageRiskDto BuildStudentAverageRiskDTO(
        int StudentId = 1,
        string StudentName = "Andres Soto",
        string Email = "andres.soto@test.com"
    )
        => new () {
            StudentId = StudentId,
            StudentName = StudentName,
            Email = Email,
            AvgRiskLevel = 3
        };

    [Fact]
    public async Task Handler_ShouldReturnSuccessResponse()
    {
        List<StudentAverageRiskDto> responseItems =
            [
                BuildStudentAverageRiskDTO(),
                BuildStudentAverageRiskDTO(2, "Carla Buendia", "carla.buendia@test.com")
            ];
        var response = new PagedResult<StudentAverageRiskDto>(responseItems.Count, responseItems);

        _mockStudentRepository.Setup(Repo => Repo.GetStudentAverageRiskByCohortsAsync(
            It.IsAny<Pagination>(),
            It.IsAny<List<int>>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<int?>()
        )).ReturnsAsync(response);

        var query = new GetAllAverageRiskByCohortAndPollQuery(
            new Pagination(),
            [1, 2],
            "mock-Uu1D",
            true,
            1
        );

        PagedResult<StudentAverageRiskDto> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(response.Count, result.Count);
        Assert.Equal(response.Items.Count, result.Items.Count);
        Assert.Collection(result.Items,
            item => Assert.Equal(response.Items[0].StudentName, item.StudentName),
            item => Assert.Equal(response.Items[1].StudentName, item.StudentName)
        );
    }

    [Fact]
    public async Task Handler_ShouldReturnEmptyPagedResult_WhenNoItemsInRepo()
    {
        var response = new PagedResult<StudentAverageRiskDto>(0, []);

        _mockStudentRepository.Setup(Repo => Repo.GetStudentAverageRiskByCohortsAsync(
            It.IsAny<Pagination>(),
            It.IsAny<List<int>>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<int?>()
        )).ReturnsAsync(response);

        var query = new GetAllAverageRiskByCohortAndPollQuery(
            new Pagination(),
            [1, 2],
            "mock-Uu1D",
            true,
            1
        );

        PagedResult<StudentAverageRiskDto> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(response.Count, result.Count);
        Assert.Empty(result.Items);
    }
}
