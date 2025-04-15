using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries
{
    public class GetAllStudentsQueryTest
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<ILogger<GetAllStudentsQueryHandler>> _mockLogger;
        private readonly GetAllStudentsQueryHandler _handler;

        public GetAllStudentsQueryTest()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockLogger = new Mock<ILogger<GetAllStudentsQueryHandler>>();
            _handler = new GetAllStudentsQueryHandler(_mockStudentRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_ResponseAsync()
        {
            // Arrange
            var query = new GetAllStudentsQuery(new Utils.Pagination());
            List<Student> students = new List<Student>()
            {
                new Student(){Email = "StudentEmail1"},
                new Student(){Email = "StudentEmail2"}
            };

            _mockStudentRepository
                .Setup(Repo => Repo.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(students);

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(2,result.Items.Count);
        }
    }
}
