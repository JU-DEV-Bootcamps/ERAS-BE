using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Students.Queries.GetStudentDetails;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students.Queries;
public class GetStudentDetailsQueryHandlerTest
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IStudentDetailRepository> _mockStudentDetailRepository;
    private readonly Mock<ILogger<GetStudentDetailsQueryHandler>> _mocklogger;
    private readonly GetStudentDetailsQueryHandler _handler;
    public GetStudentDetailsQueryHandlerTest()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockStudentDetailRepository = new Mock<IStudentDetailRepository>();
        _mocklogger = new Mock<ILogger<GetStudentDetailsQueryHandler>>();
        _handler = new GetStudentDetailsQueryHandler(
            _mockStudentRepository.Object,
            _mocklogger.Object,
            _mockStudentDetailRepository.Object
        );
    }

    private static Student BuildStudent(int Id = 1, string Name = "Mario Asturias")
        => new() { Id = Id, Name = Name };

    private static StudentDetail BuildStudentDetail(int StudentId = 1)
        => new() {
            StudentId = StudentId, Audit = new Domain.Common.AuditInfo() };

    [Fact]
    public async Task Handler_ShouldReturnSuccessCommandResponse()
    {
        Student student = BuildStudent();
        StudentDetail studentDetail = BuildStudentDetail();

        _mockStudentRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(student);
        _mockStudentDetailRepository.Setup(Repo => Repo.GetByStudentId(It.IsAny<int>()))
            .ReturnsAsync(studentDetail);
        
        var query = new GetStudentDetailsQuery { StudentId = 1 };

        CreateCommandResponse<Student> response = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(response.Entity);
        Assert.Equal(student.Id, response.Entity.Id);
        Assert.Equal(studentDetail.StudentId, response.Entity.StudentDetail.StudentId);
        Assert.True(response.Success);
        Assert.Equal("Success", response.Message);
    }

    [Fact]
    public async Task Handler_ShouldReturnErrorResponse_WhenStudentNotFound()
    {
        _mockStudentRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(value: null);
        
        var query = new GetStudentDetailsQuery { StudentId = 1 };

        CreateCommandResponse<Student> response = await _handler.Handle(query, CancellationToken.None);

        _mockStudentDetailRepository.Verify(Repo => Repo.GetByStudentId(It.IsAny<int>()), Times.Never);
        Assert.Null(response.Entity);
        Assert.False(response.Success);
        Assert.Equal("Student Not Found", response.Message);
    }

    [Fact]
    public async Task Handler_ResponseStudentDetailsShouldBeNull_WhenStudentDetailsNotFound()
    {
        Student student = BuildStudent();

        _mockStudentRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(student);
        _mockStudentDetailRepository.Setup(Repo => Repo.GetByStudentId(It.IsAny<int>()))
            .ReturnsAsync(value: null);
        
        var query = new GetStudentDetailsQuery { StudentId = 1 };

        CreateCommandResponse<Student> response = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(response.Entity);
        Assert.Equal(student.Id, response.Entity.Id);
        Assert.Null(response.Entity.StudentDetail);
        Assert.True(response.Success);
        Assert.Equal("Success", response.Message);
    }

    [Fact]
    public async Task Handler_ShouldReturnErrorResponse_WhenExceptionIsThrown()
    {
        _mockStudentRepository.Setup(Repo => Repo.GetByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("DB Error."));
        
        var query = new GetStudentDetailsQuery { StudentId = 1 };

        CreateCommandResponse<Student> response = await _handler.Handle(query, CancellationToken.None);

        _mockStudentDetailRepository.Verify(Repo => Repo.GetByStudentId(It.IsAny<int>()), Times.Never);
        Assert.Null(response.Entity);
        Assert.False(response.Success);
        Assert.Equal("Error", response.Message);
    }
}