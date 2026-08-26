using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries;
public class GetStudentByEmailQueryHandlerTest
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ILogger<GetStudentByEmailQueryHandler>> _mockLogger;
    private readonly GetStudentByEmailQueryHandler _handler;

    public GetStudentByEmailQueryHandlerTest()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<GetStudentByEmailQueryHandler>>();
        _handler = new GetStudentByEmailQueryHandler(_mockStudentRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handlee_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new GetStudentByEmailQuery() {
            studentEmail = "StudentTest"
        };
        var studentExpected = new Student()
        {
            Name = "Student",
            Email = "StudentTest"
        };
        Student studentUnexpected = new Student()
        {
            Name = "Student2",
            Email = "StudentTest2"
        };

        _mockStudentRepository
            .Setup(Repo => Repo.GetByEmailAsync(It.Is<string>(Email => Email == "StudentTest")))
            .ReturnsAsync(studentExpected);
        _mockStudentRepository
            .Setup(Repo => Repo.GetByEmailAsync(It.Is<string>(Email => Email == "StudentTest2")))
            .ReturnsAsync(studentUnexpected);

        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Student",result.Body!.Name);
    }

    [Fact]
    public async Task Handler_ShouldThrowException_WhenStudentNotFound()
    {
        _mockStudentRepository.Setup(Repo => Repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);
        
        var query = new GetStudentByEmailQuery() {
            studentEmail = "StudentTest"
        };

        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            async () => await _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal("Student not found", exception.FriendlyMessage);
    }
}
