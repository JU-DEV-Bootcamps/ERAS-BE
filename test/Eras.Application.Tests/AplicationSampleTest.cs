﻿using Eras.Application.Contracts.Persistence;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests
{
    public class AplicationSampleTest
    {
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<ILogger<StudentService>> _loggerMock;
        private readonly StudentService _studentService;

        public AplicationSampleTest()
        {
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _loggerMock = new Mock<ILogger<StudentService>>();
            _studentService = new StudentService(_studentRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateStudentShouldReturnStudentWhenStudentIsValidAsync()
        {
            // Arrange
            var student = new Student { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
            _studentRepositoryMock.Setup(Repo => Repo.AddAsync(student)).ReturnsAsync(student);

            // Act
            var result = await _studentService.CreateStudent(student);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(student.Id, result.Id);
            Assert.Equal(student.Name, result.Name);
            Assert.Equal(student.Email, result.Email);
            _studentRepositoryMock.Verify(Repo => Repo.AddAsync(student), Times.Once);
        }
    }
}
