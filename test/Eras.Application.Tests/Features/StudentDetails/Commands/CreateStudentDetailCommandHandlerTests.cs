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
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.StudentDetails.Commands
{
    public class CreateStudentDetailCommandHandlerTests
    {
        private readonly Mock<IStudentDetailRepository> _mockStudentDetailRepository;
        private readonly Mock<ILogger<CreateStudentDetailCommandHandler>> _mockLogger;
        private readonly CreateStudentDetailCommandHandler _handler;

        public CreateStudentDetailCommandHandlerTests()
        {
            _mockStudentDetailRepository = new Mock<IStudentDetailRepository>();
            _mockLogger = new Mock<ILogger<CreateStudentDetailCommandHandler>>();
            _handler = new CreateStudentDetailCommandHandler(_mockStudentDetailRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_StudentDetail_CreatesNewStudentDetail()
        {
            var newStudentDetailDto = new StudentDetailDTO() { StudentId = 1010 };
            var command = new CreateStudentDetailCommand { StudentDetailDto = newStudentDetailDto };
            var newStudentDetail = newStudentDetailDto.ToDomain;

            _mockStudentDetailRepository.Setup(repo => repo.AddAsync(It.IsAny<StudentDetail>()))
                .ReturnsAsync(newStudentDetail);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1010, result.Entity.StudentId);
        }

    }
}
