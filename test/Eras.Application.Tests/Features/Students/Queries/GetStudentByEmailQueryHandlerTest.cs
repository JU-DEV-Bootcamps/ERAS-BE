using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries
{
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
        public async Task Handle_Should_Return_Success_ResponseAsync()
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
            Assert.Equal("Student",result.Body.Name);
        }
    }
}
