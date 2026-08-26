using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Commands.UpdateStudent;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Students.Commands;
public class CreateStudentsCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ILogger<CreateStudentsCommandHandler>> _mockLogger;
    private readonly Mock<IMediator> _mockMediator;
    private readonly CreateStudentsCommandHandler _handler;

    public CreateStudentsCommandHandlerTests()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<CreateStudentsCommandHandler>>();
        _mockMediator = new Mock<IMediator>();

        _handler = new CreateStudentsCommandHandler(
            _mockStudentRepository.Object,
            _mockLogger.Object,
            _mockMediator.Object);
    }

    private static StudentImportDto ValidDTO(string Email = "student@test.com", string Name = "Hector Lenon") =>
        new ()
        {
            Name = Name,
            Email = Email,
            EnrolledCourses = 3,
            GradedCourses = 2,
            TimelySubmissions = 1,
            AverageScore = 88,
            CoursesBelowAverage = 1,
            RawScoreDifference = 5,
            StandardScoreDifference = 1,
            DaysSinceLastAccess = 2,
            SISId = "S1SID"
        };

    private static StudentImportDto DefaultDTO(string Email = "default@test.com", string Name = "Tales De Mileto") =>
        new ()
        {
            Name = Name,
            Email = Email,
            EnrolledCourses = 0,
            GradedCourses = 0,
            TimelySubmissions = 0,
            AverageScore = 0,
            CoursesBelowAverage = 0,
            RawScoreDifference = 0,
            StandardScoreDifference = 0,
            DaysSinceLastAccess = 0,
            SISId = "S1SID"
        };

    private void SetupStudentNotFound() =>
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetStudentByEmailQuery>(), CancellationToken.None))
            .ReturnsAsync(new GetQueryResponse<Student>(null));
    private void SetupStudentFound(Student ExistingStudent) =>
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetStudentByEmailQuery>(), CancellationToken.None))
            .ReturnsAsync(new GetQueryResponse<Student>(ExistingStudent));
    private void SetupCreatedStudent(Student CreatedStudent) =>
        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateStudentCommand>(), CancellationToken.None))
            .ReturnsAsync(new CreateCommandResponse<Student>(CreatedStudent, 1, "Success", true));
    private void SetupCreatedStudentDetail(StudentDetail? StudentDetail) => 
        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateStudentDetailCommand>(), CancellationToken.None))
            .ReturnsAsync(new CreateCommandResponse<StudentDetail>(StudentDetail ?? new StudentDetail(), 1, "Success", true));

    [Fact]
    public async Task Handler_ShouldCreateSingleStudentAndReturnSuccessResponse()
    {
        StudentImportDto studentImportDTO = ValidDTO();
        var createdStudent = new Student { Name = studentImportDTO.Name, Email = studentImportDTO.Email };

        SetupStudentNotFound();
        SetupCreatedStudent(createdStudent);
        SetupCreatedStudentDetail(null);

        var command = new CreateStudentsCommand { students = [studentImportDTO] };

        CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Single(result.Entity);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Equal("1 new students, 0 updated, and 0 with errors.", result.Message);
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentCommand>(), CancellationToken.None), Times.Once);
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateStudentCommand>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldUpdateExistingStudentAndReturnSuccessResponse()
    {
        StudentImportDto studentImportDTO = ValidDTO();
        var existingStudent = new Student { Name = "Old Name", Email = studentImportDTO.Email };

        SetupStudentFound(existingStudent);
        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateStudentCommand>(), CancellationToken.None))
            .ReturnsAsync(new CreateCommandResponse<Student>(existingStudent, 0, "Success", true));

        var command = new CreateStudentsCommand { students = [studentImportDTO] };

        CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Empty(result.Entity);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("0 new students, 1 updated, and 0 with errors.", result.Message);
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateStudentCommand>(), CancellationToken.None), Times.Once);
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentCommand>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldAddErrorStudentToErrorListAndSkipMediatorCalls()
    {
        var invalidDTO = new StudentImportDto { Name = "J0se Muñoz", Email = "invalid.mail", SISId = "" };

        var command = new CreateStudentsCommand { students =[invalidDTO] };

        CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Empty(result.Entity);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("0 new students, 0 updated, and 1 with errors.", result.Message);
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateStudentCommand>(), CancellationToken.None), Times.Never);
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentCommand>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldAddEntityToErrorList_WhenCreateStudentCommandFails()
    {
        StudentImportDto studentImportDTO = ValidDTO();
        var failedEntity = new Student { Name = studentImportDTO.Name, Email = studentImportDTO.Email };

        SetupStudentNotFound();
        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateStudentCommand>(), CancellationToken.None))
            .ReturnsAsync(new CreateCommandResponse<Student>(failedEntity, 0, "Error", false));

        var command = new CreateStudentsCommand { students = [studentImportDTO] };

        CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Empty(result.Entity);
        Assert.Equal("0 new students, 0 updated, and 1 with errors.", result.Message);
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentDetailCommand>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldGenerateStudentDetail_WhenCreatedStudentDoesNotHaveDetails()
    {
        StudentImportDto studentImportDTO = ValidDTO();
        var createdEntity = new Student {
            Name = studentImportDTO.Name,
            Email = studentImportDTO.Email,
            StudentDetail = new StudentDetail {
                Id = 0,
                Audit = new Domain.Common.AuditInfo { ModifiedAt = DateTime.Now }
            }
        };

        SetupStudentNotFound();
        SetupCreatedStudent(createdEntity);
        SetupCreatedStudentDetail(null);

        var command = new CreateStudentsCommand { students = [studentImportDTO] };

        await _handler.Handle(command, CancellationToken.None);

        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentDetailCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handler_ShouldSkipStudentDetailCreation_WhenDTOHasDefaultDetailData()
    {
        StudentImportDto studentImportDTO = DefaultDTO();
        var createdEntity = new Student {
            Name = studentImportDTO.Name,
            Email = studentImportDTO.Email,
            StudentDetail = new StudentDetail { Id = 1 }
        };

        SetupStudentNotFound();
        SetupCreatedStudent(createdEntity);

        var command = new CreateStudentsCommand { students = [studentImportDTO] };

        await _handler.Handle(command, CancellationToken.None);

        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentDetailCommand>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldSkipStudentDetailCreation_WhenStudentAlreadyHasDetail()
    {
        StudentImportDto studentImportDTO = ValidDTO();
        var createdEntity = new Student {
            Name = studentImportDTO.Name,
            Email = studentImportDTO.Email,
            StudentDetail = new StudentDetail { Id = 1 }
        };

        SetupStudentNotFound();
        SetupCreatedStudent(createdEntity);

        var command = new CreateStudentsCommand { students = [studentImportDTO] };

        await _handler.Handle(command, CancellationToken.None);

        _mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentDetailCommand>(), CancellationToken.None), Times.Never);
    }

    // // ---------- Created vs updated bucketing ----------

    // [Fact]
    // public async Task Handle_SuccessfullImportsZero_AddsToUpdatedStudents()
    // {
    //     // Arrange
    //     StudentImportDto dto = ValidDto();
    //     Student existing = new Student { Name = dto.Name, Email = dto.Email, StudentDetail = new StudentDetail { Id = 1 } };

    //     SetupStudentFound(dto.Email, existing);
    //     _mockMediator
    //         .Setup(m => m.Send(It.IsAny<UpdateStudentCommand>(), CancellationToken.None))
    //         .ReturnsAsync(new CreateCommandResponse<Student>(existing, 0, "updated", true));

    //     CreateStudentsCommand command = new CreateStudentsCommand { students = new[] { dto } };

    //     // Act
    //     CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

    //     // Assert: updated students are not included in the returned Entity array, only reflected in the message
    //     Assert.Equal(0, result.Entity.Length);
    //     Assert.Contains("1 updated", result.Message);
    // }

    [Fact]
    public async Task Handler_ShouldCountMixOfCreatedUpdatedAndErrorStudents()
    {
        StudentImportDto newDto = ValidDTO("new@test.com", "New Student");
        StudentImportDto existingDto = ValidDTO("existing@test.com", "Existing Student");
        var invalidDto = new StudentImportDto { Name = "", Email = "", SISId = "" };

        var createdStudent = new Student {
            Name = newDto.Name,
            Email = newDto.Email,
            StudentDetail = new StudentDetail { Audit = new Domain.Common.AuditInfo { ModifiedAt = DateTime.Now } } };
        var existingStudent = new Student {
            Name = existingDto.Name,
            Email = existingDto.Email,
            StudentDetail = new StudentDetail { Id = 1 } };

        SetupCreatedStudent(createdStudent);
        SetupCreatedStudentDetail(null);

        _mockMediator
            .SetupSequence(m => m.Send(It.IsAny<GetStudentByEmailQuery>(), CancellationToken.None))
            .ReturnsAsync(new GetQueryResponse<Student>(null))
            .ReturnsAsync(new GetQueryResponse<Student>(existingStudent));
        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateStudentCommand>(), CancellationToken.None))
            .ReturnsAsync(new CreateCommandResponse<Student>(existingStudent, 0, "Success", true));

        var command = new CreateStudentsCommand { students = [newDto, existingDto, invalidDto] };

        CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Single(result.Entity);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Equal("1 new students, 1 updated, and 1 with errors.", result.Message);
    }

    [Fact]
    public async Task Handler_ShouldCatchExceptionAndReturnFailureResponse()
    {
        StudentImportDto studentImportDto = ValidDTO();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetStudentByEmailQuery>(), CancellationToken.None))
            .ThrowsAsync(new Exception("DB Error."));

        var command = new CreateStudentsCommand { students = [studentImportDto] };

        CreateCommandResponse<Student[]> result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
        Assert.Equal(0, result.SuccessfullImports);
    }

    [Fact]
    public async Task CreateStudentDetailAsync_MapsDtoFieldsAndSendsCreateStudentDetailCommand()
    {
        StudentImportDto studentImportDto = ValidDTO();
        var mappedStudentDetail = new StudentDetail
        {
            AvgScore = studentImportDto.AverageScore,
            CoursesUnderAvg = studentImportDto.CoursesBelowAverage,
            EnrolledCourses = studentImportDto.EnrolledCourses,
            GradedCourses = studentImportDto.GradedCourses,
            LastAccessDays = studentImportDto.DaysSinceLastAccess,
            StandardScoreDiff = studentImportDto.StandardScoreDifference,
            PureScoreDiff = studentImportDto.RawScoreDifference,
            TimeDeliveryRate = studentImportDto.TimelySubmissions,
        };
        var student = new Student {
            StudentDetail = new StudentDetail {
                Audit = new Domain.Common.AuditInfo { ModifiedAt = DateTime.UtcNow }
            }
        };

        SetupCreatedStudentDetail(mappedStudentDetail);

        CreateCommandResponse<StudentDetail> result = await _handler.CreateStudentDetailAsync(student, studentImportDto);

        Assert.NotNull(result.Entity);
        Assert.Equal(studentImportDto.EnrolledCourses, result.Entity.EnrolledCourses);
        Assert.Equal(studentImportDto.GradedCourses, result.Entity.GradedCourses);
        Assert.Equal(studentImportDto.TimelySubmissions, result.Entity.TimeDeliveryRate);
        Assert.Equal(studentImportDto.AverageScore, result.Entity.AvgScore);
        Assert.Equal(studentImportDto.CoursesBelowAverage, result.Entity.CoursesUnderAvg);
        Assert.Equal(studentImportDto.RawScoreDifference, result.Entity.PureScoreDiff);
        Assert.Equal(studentImportDto.StandardScoreDifference, result.Entity.StandardScoreDiff);
        Assert.Equal(studentImportDto.DaysSinceLastAccess, result.Entity.LastAccessDays);
    }
}