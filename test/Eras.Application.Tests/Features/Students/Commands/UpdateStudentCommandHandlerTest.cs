using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.UpdateStudent;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Commands;
public class UpdateStudentCommandHandlerTest
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ILogger<UpdateStudentCommandHandler>> _mockLogger;
    private readonly UpdateStudentCommandHandler _handler;
    public UpdateStudentCommandHandlerTest()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<UpdateStudentCommandHandler>>();
        _handler = new UpdateStudentCommandHandler(_mockStudentRepository.Object, _mockLogger.Object);
    }

    private static StudentDTO BuildStudentDTO(
        int Id = 1,
        string Uuid = "m0ck-Uu1D",
        string Name = "Jimena Lopez",
        string Email = "jimena.l@test.com"
    )
        => new ()
        {
            Id = Id,
            Uuid = Uuid,
            Name = Name,
            Email = Email
        };

    [Fact]
    public async Task Handler_ShouldUpdateStudentAndReturnSuccessResponse()
    {
        StudentDTO studentDTO = BuildStudentDTO();
        Student existingStudent = studentDTO.ToDomain();
        Student updatedStudent = studentDTO.ToDomain();
        updatedStudent.IsImported = true;

        _mockStudentRepository.Setup(Repo => Repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingStudent);
        _mockStudentRepository.Setup(Repo => Repo.UpdateAsync(It.IsAny<Student>()))
            .ReturnsAsync(updatedStudent);
        
        var command = new UpdateStudentCommand { StudentDTO = studentDTO };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal(updatedStudent.IsImported, result.Entity.IsImported);
        Assert.True(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
    }

    [Fact]
    public async Task Handler_ShouldReturnErrorResponse_WhenCommandDTOIsNull()
    {
        var command = new UpdateStudentCommand { StudentDTO = null };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        _mockStudentRepository.Verify(Repo => Repo.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockStudentRepository.Verify(Repo => Repo.UpdateAsync(It.IsAny<Student>()), Times.Never);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task Handler_ShouldReturnErrorResponse_WhenStudentDoesNotExist()
    {
        StudentDTO studentDTO = BuildStudentDTO();

        _mockStudentRepository.Setup(Repo => Repo.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);

        var command = new UpdateStudentCommand { StudentDTO = studentDTO };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        _mockStudentRepository.Verify(Repo => Repo.UpdateAsync(It.IsAny<Student>()), Times.Never);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Student Not Found", result.Message);
    }

    [Fact]
    public async Task Handler_ShouldCatchExceptionAndReturnErrorResponse()
    {
        StudentDTO studentDTO = BuildStudentDTO();

        _mockStudentRepository.Setup(Repo => Repo.GetByEmailAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("DB Error."));
    
        var command = new UpdateStudentCommand { StudentDTO = studentDTO };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        _mockStudentRepository.Verify(Repo => Repo.UpdateAsync(It.IsAny<Student>()), Times.Never);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
    }
}