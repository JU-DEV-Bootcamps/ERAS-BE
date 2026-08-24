using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Students.Queries;
public class GetAllStudentsByPollUuidAndDaysQueryHandlerTest
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler>> _mockLogger;
    private readonly GetAllStudentsByPollUuidAndDaysQueryHandler _handler;

    public GetAllStudentsByPollUuidAndDaysQueryHandlerTest()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<GetAllStudentsByPollUuidAndDaysQueryHandler>>();
        _handler = new GetAllStudentsByPollUuidAndDaysQueryHandler(_mockStudentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handler_ShouldReturnSuccessResponse()
    {
        var query = new GetAllStudentsByPollUuidAndDaysQuery()
        {
            Query = new Pagination(),
            PollUuid = "1",
            Days = 1
        };
        var students = new List<Student>()
            {
                new (){Email = "StudentEmail1@mail.com",},
                new (){Email = "StudentEmail2@mail.com"}
            };
        (List<Student> students, int) response = (students,2);
    
        _mockStudentRepository
            .Setup(Repo => Repo.GetAllStudentsByPollUuidAndDaysQuery(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int?>()
            ))
            .ReturnsAsync(response);

        PagedResult<Student> result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.Equal(2,result.Count);
        Assert.Equal(2,result.Items.Count);
        Assert.Collection(result.Items,
            item => Assert.Equal(students[0].Email, item.Email),
            item => Assert.Equal(students[1].Email, item.Email)
        );
    }

    [Fact]
        public async Task Handler_ShouldCatchExceptionAndReturnEmptyResponse()
        {
            var query = new GetAllStudentsByPollUuidAndDaysQuery()
            {
                Query = new Pagination(),
                PollUuid = "1",
                Days = 1
            };

            _mockStudentRepository
            .Setup(Repo => Repo.GetAllStudentsByPollUuidAndDaysQuery(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int?>()
            ))
            .ThrowsAsync(new Exception("DB Error."));

            PagedResult<Student> result = await _handler.Handle(query, CancellationToken.None);

            Assert.Empty(result.Items);
            Assert.Equal(0,result.Items.Count);
        }
}
