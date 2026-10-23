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
        public async Task HandleStudentDetailCreatesNewStudentDetailAsync()
        {
            var newStudentDetailDto = new StudentDetailDTO() { StudentId = 1010 };
            var command = new CreateStudentDetailCommand { StudentDetailDto = newStudentDetailDto };
            var newStudentDetail = newStudentDetailDto.ToDomain;

            _mockStudentDetailRepository.Setup(Repo => Repo.AddAsync(It.IsAny<StudentDetail>()))
                .ReturnsAsync(newStudentDetail);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1010, result.Entity?.StudentId);
        }

        [Fact]
        public async Task HandleStudentDetail_WithoutStudentDetailDto()
        {
            var command = new CreateStudentDetailCommand { };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Null(result.Entity);
            Assert.Equal(0, result.SuccessfullImports);
            Assert.Equal("Error", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task HandleStudentDetailUpdatesNewStudentDetailAsync_Existent()
        {
            var studentDetailDto = new StudentDetailDTO() { StudentId = 1010 };
            var command = new CreateStudentDetailCommand { StudentDetailDto = studentDetailDto };
            var studentDetailDomain = studentDetailDto.ToDomain;
            var studentDetail= new StudentDetail
            {
                StudentId = 1010,
                EnrolledCourses = 5,
                GradedCourses = 5,
                TimeDeliveryRate = 20,
                AvgScore = 2.5m,
                CoursesUnderAvg = 5,
                PureScoreDiff = 3,
                StandardScoreDiff = 4.5m,
                LastAccessDays = 3,
                Audit = new Domain.Common.AuditInfo
                {
                    ModifiedAt = DateTime.Now,
                }
            };

            _mockStudentDetailRepository.Setup(Repo => Repo.GetByStudentId(It.IsAny<int>()))
                .ReturnsAsync(studentDetail);

            _mockStudentDetailRepository.Setup(Repo => Repo.UpdateAsync(It.IsAny<StudentDetail>()))
                .ReturnsAsync(studentDetail);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(0, result.SuccessfullImports);
            Assert.Equal(studentDetail.EnrolledCourses, result.Entity!.EnrolledCourses);
            Assert.Equal(studentDetail.GradedCourses, result.Entity!.GradedCourses);
            Assert.Equal(studentDetail.TimeDeliveryRate, result.Entity!.TimeDeliveryRate);
            Assert.Equal(studentDetail.PureScoreDiff, result.Entity!.PureScoreDiff);
            Assert.Equal(studentDetail.AvgScore, result.Entity!.AvgScore);
        }
    }
}
