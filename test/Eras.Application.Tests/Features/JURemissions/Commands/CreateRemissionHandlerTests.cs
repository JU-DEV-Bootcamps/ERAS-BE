using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Remmisions.Commands.CreateRemission;
using Eras.Application.Models.Enums;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.JURemissions.Commands;

public sealed class CreateRemissionCommandHandlerTests
{
    private readonly Mock<IRemissionRepository> _remissionRepository;
    private readonly Mock<IStudentRepository> _studentRepository;
    private readonly Mock<ILogger<CreateRemissionCommandHandler>> _logger;
    private readonly CreateRemissionCommandHandler _handler;

    public CreateRemissionCommandHandlerTests()
    {
        _remissionRepository = new Mock<IRemissionRepository>();
        _studentRepository = new Mock<IStudentRepository>();
        _logger = new Mock<ILogger<CreateRemissionCommandHandler>>();
        _handler = new CreateRemissionCommandHandler(
            _remissionRepository.Object, _studentRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_WhenRemissionAlreadyExists_ReturnsAlreadyExists()
    {
        // Arrange
        var remission = CreateRemission();

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ReturnsAsync(remission);

        var command = CreateCommand(remission);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Entity already exists", result.Message);
        Assert.Equal(
            CommandEnums.CommandResultStatus.AlreadyExists,
            result.Status);

        _remissionRepository.Verify(
            x => x.AddAsync(It.IsAny<JURemission>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Student>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRemissionDoesNotExist_CreatesRemissionAndUpdatesStudents()
    {
        // Arrange
        var remission = CreateRemission(1, 2);
        var student1 = CreateStudent(1);
        var student2 = CreateStudent(2);

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ReturnsAsync((JURemission?)null);

        _remissionRepository
            .Setup(x => x.AddAsync(It.IsAny<JURemission>()))
            .ReturnsAsync(remission);

        _studentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(student1);

        _studentRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(student2);

        var command = new CreateRemissionCommand
        {
            Remission = new JURemissionDTO()
            {
                Id = remission.Id,
                StudentIds = [1, 2],
                Audit = new Domain.Common.AuditInfo()
            },
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Equal(remission, result.Entity);
    }

    [Fact]
    public async Task Handle_WhenStudentDoesNotExist_StillCreatesRemissionAndDoesNotUpdateMissingStudent()
    {
        // Arrange
        var remission = CreateRemission(1, 999);
        var existingStudent = CreateStudent(1);

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ReturnsAsync((JURemission?)null);

        _remissionRepository
            .Setup(x => x.AddAsync(It.IsAny<JURemission>()))
            .ReturnsAsync(remission);

        _studentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingStudent);

        _studentRepository
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Student?)null);

        var command = new CreateRemissionCommand
        {
            Remission = new JURemissionDTO()
            {
                Id = remission.Id,
                StudentIds = [1, 999],
                Audit = new Domain.Common.AuditInfo()
            },
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);

        Assert.Contains(remission.Id, existingStudent.RemissionIds);

        _studentRepository.Verify(
            x => x.UpdateAsync(existingStudent),
            Times.Once);

        _studentRepository.Verify(x => x.UpdateAsync(It.Is<Student>(s => s.Id == 999)), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRemissionHasNoStudents_CreatesRemissionWithoutUpdatingStudents()
    {
        // Arrange
        var remission = CreateRemission();

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ReturnsAsync((JURemission?)null);

        _remissionRepository
            .Setup(x => x.AddAsync(It.IsAny<JURemission>()))
            .ReturnsAsync(remission);

        var command = new CreateRemissionCommand
        {
            Remission = new JURemissionDTO()
            {
                Id = remission.Id,
                Audit = new Domain.Common.AuditInfo()
            },
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);

        _remissionRepository.Verify(
            x => x.AddAsync(It.IsAny<JURemission>()),
            Times.Once);

        _studentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Student>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsErrorResponse()
    {
        // Arrange
        var remission = CreateRemission();

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ThrowsAsync(new Exception("Database error"));

        var command = CreateCommand(remission);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
        Assert.Equal(
            CommandEnums.CommandResultStatus.Error,
            result.Status);

        _remissionRepository.Verify(
            x => x.AddAsync(It.IsAny<JURemission>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAddingRemissionThrows_ReturnsErrorResponse()
    {
        // Arrange
        var remission = CreateRemission(1);

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ReturnsAsync((JURemission?)null);

        _remissionRepository
            .Setup(x => x.AddAsync(It.IsAny<JURemission>()))
            .ThrowsAsync(new Exception("Database error"));

        var command = CreateCommand(remission);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
        Assert.Equal(
            CommandEnums.CommandResultStatus.Error,
            result.Status);

        _studentRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdatingStudentThrows_ReturnsErrorResponse()
    {
        // Arrange
        var remission = CreateRemission(1);
        var student = CreateStudent(1);

        _remissionRepository
            .Setup(x => x.GetByIdAsync(remission.Id))
            .ReturnsAsync((JURemission?)null);

        _remissionRepository
            .Setup(x => x.AddAsync(It.IsAny<JURemission>()))
            .ReturnsAsync(remission);

        _studentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(student);

        _studentRepository
            .Setup(x => x.UpdateAsync(student))
            .ThrowsAsync(new Exception("Database error"));

        var command = CreateCommand(remission);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
        Assert.Equal(
            CommandEnums.CommandResultStatus.Error,
            result.Status);
    }

    private static CreateRemissionCommand CreateCommand(JURemission remission)
        => new ()
        {
            Remission = new JURemissionDTO()
            {
                Id = remission.Id,
            }
        };

    private static JURemission CreateRemission(params int[] studentIds)
        => new()
        {
            Id = 1,
            StudentIds = studentIds.ToList()
        };

    private static Student CreateStudent(int id)
        => new()
        {
            Id = id,
            RemissionIds = new List<int>()
        };
}
