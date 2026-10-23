using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Students.Commands
{
    public class CreateStudentCommandHandlerTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<ILogger<CreateStudentCommandHandler>> _mockLogger;
        private readonly CreateStudentCommandHandler _handler;

        public CreateStudentCommandHandlerTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockLogger = new Mock<ILogger<CreateStudentCommandHandler>>();
            _handler = new CreateStudentCommandHandler(_mockStudentRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handler_ShouldCreatesNewStudentAndReturnSuccessResponse()
        {
            var newStudentDto = new StudentDTO() { Name= "newStudent" };
            var command = new CreateStudentCommand { StudentDTO = newStudentDto };
            Student newStudent = newStudentDto.ToDomain();

            _mockStudentRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Student>()))
                .ReturnsAsync(newStudent);

            CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result.Entity);
            Assert.Equal("newStudent", result.Entity.Name);
        }

        [Fact]
        public async Task Handler_ShouldReturnFailureResponse_WhenStudentAlreadyExists()
        {
            var studentDTO = new StudentDTO { Email = "student@test.com" };
            Student existingStudent = studentDTO.ToDomain();

            _mockStudentRepository.Setup(Repo => Repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(existingStudent);

            var command = new CreateStudentCommand { StudentDTO = studentDTO };

            CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(result.Entity);
            Assert.False(result.Success);
            Assert.Equal("Student Already Exists", result.Message);
        }

        [Fact]
        public async Task Handler_ShouldCatchExceptionAndReturnErrorResponse()
        {
            var studentDTO = new StudentDTO { Email = "student@test.com" };

            _mockStudentRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Student>()))
                .ThrowsAsync(new Exception("DB Error."));

            var command = new CreateStudentCommand { StudentDTO = studentDTO };

            CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(result.Entity);
            Assert.False(result.Success);
            Assert.Equal("Error", result.Message);
        }
    }
}
