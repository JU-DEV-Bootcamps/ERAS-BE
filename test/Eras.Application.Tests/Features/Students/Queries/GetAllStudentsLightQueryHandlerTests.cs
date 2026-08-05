using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Students.Queries.GetAllLight;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries
{
    public class GetAllStudentsLightQueryHandlerTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<ILogger<GetAllStudentsLightQueryHandler>> _mockLogger;
        private readonly GetAllStudentsLightQueryHandler _handler;

        public GetAllStudentsLightQueryHandlerTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockLogger = new Mock<ILogger<GetAllStudentsLightQueryHandler>>();
            _handler = new GetAllStudentsLightQueryHandler(_mockStudentRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_AllLightStudents()
        {
            var query = new GetAllStudentsLightQuery();
            IEnumerable<StudentLightDto> students = new List<StudentLightDto>
            {
                new() { Id = 1, Name = "Ana" },
                new() { Id = 2, Name = "Beto" },
            };

            _mockStudentRepository
                .Setup(repo => repo.GetAllLightAsync())
                .ReturnsAsync(students);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Equal("Ana", result[0].Name);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_WhenRepositoryThrows()
        {
            var query = new GetAllStudentsLightQuery();

            _mockStudentRepository
                .Setup(repo => repo.GetAllLightAsync())
                .ThrowsAsync(new Exception("DB error"));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Empty(result);
        }
    }
}