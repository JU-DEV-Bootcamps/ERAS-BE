using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
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
        public async Task Handler_ShouldReturnSuccessResponse()
        {
            var query = new GetAllStudentsQuery(new Pagination());
            var students = new List<Student>()
            {
                new() {Email = "StudentEmail1"},
                new() {Email = "StudentEmail2"}
            };

            _mockStudentRepository
                .Setup(Repo => Repo.GetPagedAsyncWithJoins(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(students);

            PagedResult<GetAllStudentsQueryResponse> result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result.Items);
            Assert.Equal(2,result.Items.Count);
            Assert.Collection(result.Items,
                item => Assert.Equal(students[0].Email, item.Email),
                item => Assert.Equal(students[1].Email, item.Email)
            );
        }

        [Fact]
        public async Task Handler_ShouldCatchExceptionAndReturnEmptyResponse()
        {
            var query = new GetAllStudentsQuery(new Pagination());

            _mockStudentRepository.Setup(Repo => Repo.GetPagedAsyncWithJoins(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error."));
            
            PagedResult<GetAllStudentsQueryResponse> result = await _handler.Handle(query, CancellationToken.None);

            _mockStudentRepository.Verify(Repo => Repo.CountAsync(), Times.Never);
            Assert.Empty(result.Items);
            Assert.Equal(0,result.Items.Count);
        }
    }
}
