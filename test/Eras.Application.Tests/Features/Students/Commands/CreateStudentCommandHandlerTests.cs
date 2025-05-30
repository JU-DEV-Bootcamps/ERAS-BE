using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
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
        public async Task HandleStudentCreatesNewStudentAsync()
        {
            var newStudentDto = new StudentDTO() { Name= "newStudent" };
            var command = new CreateStudentCommand { StudentDTO = newStudentDto };
            var newStudent = newStudentDto.ToDomain;

            _mockStudentRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Student>()))
                .ReturnsAsync(newStudent);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("newStudent", result.Entity?.Name);
        }

    }
}
