using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Commands.DeleteStudent;
using Eras.Application.Features.Students.Queries.GetStudentProfile;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

public class DeleteStudentCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly DeleteStudentCommandHandler _handler;

    public DeleteStudentCommandHandlerTests()
    {
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync(new StudentProfile { StudentId = 7 });
        _handler = new DeleteStudentCommandHandler(_students.Object, _profiles.Object);
    }

    private Task Run() => _handler.Handle(new DeleteStudentCommand(7), CancellationToken.None);

    [Fact]
    public async Task Handle_UnknownStudent_ThrowsNotFound()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(Run);
    }

    [Fact]
    public async Task Handle_ImportedStudent_ThrowsConflictAndDeletesNothing()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(new Student { Id = 7, IsImported = true });

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(Run);

        Assert.Equal(409, error.StatusCode);
        _students.Verify(R => R.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StudentWithoutProfile_ThrowsConflictAndDeletesNothing()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(new Student { Id = 7 });
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync((StudentProfile?)null);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(Run);

        Assert.Equal(409, error.StatusCode);
        _students.Verify(R => R.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StudentWithRelatedData_ThrowsConflictAndDeletesNothing()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(new Student { Id = 7 });
        _students.Setup(R => R.HasRelatedDataAsync(7)).ReturnsAsync(true);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(Run);

        Assert.Equal(409, error.StatusCode);
        _students.Verify(R => R.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ManualStudentWithoutRelatedData_DeletesIt()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(new Student { Id = 7 });
        _students.Setup(R => R.HasRelatedDataAsync(7)).ReturnsAsync(false);

        await Run();

        _students.Verify(R => R.DeleteByIdAsync(7), Times.Once);
    }
}

public class GetStudentProfileQueryHandlerTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly GetStudentProfileQueryHandler _handler;

    public GetStudentProfileQueryHandlerTests()
    {
        _handler = new GetStudentProfileQueryHandler(_students.Object, _profiles.Object);
    }

    private Task<StudentRegistrationDto> Run() =>
        _handler.Handle(new GetStudentProfileQuery(7), CancellationToken.None);

    [Fact]
    public async Task Handle_UnknownStudent_ThrowsNotFound()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(Run);
    }

    [Fact]
    public async Task Handle_StudentWithProfile_ReturnsItWithStudentEmail()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(new Student { Id = 7, Email = "a@b.co" });
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync(new StudentProfile
        {
            StudentId = 7,
            FirstName = "Ana",
            LastName = "Pérez",
            IdPassportNumber = "X1",
            City = "Cbba",
        });

        StudentRegistrationDto result = await Run();

        Assert.Equal("Ana", result.FirstName);
        Assert.Equal("Cbba", result.City);
        Assert.Equal("a@b.co", result.PrimaryEmail);
    }

    [Fact]
    public async Task Handle_StudentWithoutProfile_SplitsTheDisplayName()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(
            new Student { Id = 7, Name = "Juan Perez Testeador", Email = "j@b.co", IsImported = true });
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync((StudentProfile?)null);

        StudentRegistrationDto result = await Run();

        Assert.Equal("Juan", result.FirstName);
        Assert.Equal("Perez Testeador", result.LastName);
        Assert.True(result.IsImported);
        Assert.Equal(string.Empty, result.IdPassportNumber);
    }
}
