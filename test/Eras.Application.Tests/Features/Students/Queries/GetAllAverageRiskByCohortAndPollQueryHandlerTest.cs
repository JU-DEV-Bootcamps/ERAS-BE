using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohortAndPoll;
using Eras.Application.Utils;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries;

public class GetAllAverageRiskByCohortAndPollQueryHandlerTest
{
    private readonly Mock<ILogger<GetAllAverageRiskByCohortAndPollQueryHandler>> _logger;
    private readonly Mock<IStudentRepository> _repository;
    private readonly GetAllAverageRiskByCohortAndPollQueryHandler _handler;

    public GetAllAverageRiskByCohortAndPollQueryHandlerTest()
    {
        _logger = new Mock<ILogger<GetAllAverageRiskByCohortAndPollQueryHandler>>();
        _repository = new Mock<IStudentRepository>();
        _handler = new GetAllAverageRiskByCohortAndPollQueryHandler(_repository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_ResponseAsync()
    {
        // Arrange
        var items = new List<StudentAverageRiskDto>
        {
            new StudentAverageRiskDto
            {
                StudentId = 1,
                StudentName = "Blair",
                Email = "blair@f.com"
            },
            new StudentAverageRiskDto
            {
                StudentId = 2,
                StudentName = "Cyan",
                Email = "cyan@f.com"
            },
        };
        var data = new PagedResult<StudentAverageRiskDto>(2, items);
        var request = new GetAllAverageRiskByCohortAndPollQuery(
            new Pagination { Page = 0, PageSize = 5 }, 
            new List<int>() { 2 }, "2", true, 32);

        _repository
            .Setup(x => x.GetStudentAverageRiskByCohortsAsync(
                It.IsAny<Pagination>(),
                It.IsAny<List<int>>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<int>())
                )
            .ReturnsAsync(data);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        var mappedStudent1 = result.Items.Single(x => x.StudentId == 1);
        Assert.Equal("Blair", mappedStudent1.StudentName);
        Assert.Equal("blair@f.com", mappedStudent1.Email);

        var mappedStudent2 = result.Items.Single(x => x.StudentId == 2);
        Assert.Equal("Cyan", mappedStudent2.StudentName);
        Assert.Equal("cyan@f.com", mappedStudent2.Email);
    }
}
