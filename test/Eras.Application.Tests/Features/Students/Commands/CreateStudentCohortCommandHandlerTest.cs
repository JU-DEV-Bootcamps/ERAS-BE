using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Commands.CreateStudentCohort;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Commands;
public class CreateStudentCohortCommandHandlerTest
{
    private readonly Mock<IStudentCohortRepository> _mockStudentCohortRepository;
    private readonly Mock<ILogger<CreateStudentCohortCommandHandler>> _mockLogger;
    private readonly CreateStudentCohortCommandHandler _handler;
    public CreateStudentCohortCommandHandlerTest()
    {
        _mockStudentCohortRepository = new Mock<IStudentCohortRepository>();
        _mockLogger = new Mock<ILogger<CreateStudentCohortCommandHandler>>();
        _handler = new CreateStudentCohortCommandHandler(_mockStudentCohortRepository.Object, _mockLogger.Object);   
    }

    [Fact]
    public async Task Handler_ShouldCreateStudentCohortAndReturnSuccessResponse()
    {
        var createdStudent = new Student
        {
            CohortId = 1,
            Id = 1
        };

        _mockStudentCohortRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Student>()))
            .ReturnsAsync(createdStudent);

        var command = new CreateStudentCohortCommand { CohortId = 1, StudentId = 1 };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal(1, result.Entity.Id);
        Assert.Equal(1, result.Entity.CohortId);
        Assert.True(result.Success);
        Assert.Equal(1, result.SuccessfullImports);
    }

    [Fact]
    public async Task Handler_ShouldReturnExistingStudent()
    {
        var existingStudent = new Student
        {
            CohortId = 1,
            Id = 1
        };

        _mockStudentCohortRepository.Setup(Repo => Repo.GetByCohortIdAndStudentIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(existingStudent);
        
        var command = new CreateStudentCohortCommand { CohortId = 1, StudentId = 1 };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        _mockStudentCohortRepository.Verify(Repo => Repo.AddAsync(It.IsAny<Student>()), Times.Never);
        Assert.NotNull(result.Entity);
        Assert.Equal(1, result.Entity.Id);
        Assert.Equal(1, result.Entity.CohortId);
        Assert.True(result.Success);
        Assert.Equal(0, result.SuccessfullImports);
    }

    [Fact]
    public async Task Handler_ShouldCatchExceptionAndReturnErrorResponse()
    {
        _mockStudentCohortRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Student>()))
            .ThrowsAsync(new Exception("DB Error."));
        
        var command = new CreateStudentCohortCommand { CohortId = 1, StudentId = 1 };

        CreateCommandResponse<Student> result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Equal(0, result.SuccessfullImports);   
        Assert.Equal("Error", result.Message);   
    }
}