using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Queries.GetStudentProfile;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.Students;

public class GetStudentProfileProfessionalScopeTests
{

    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly Mock<IStudentCohortRepository> _cohorts = new();
    private readonly Mock<IStudentAccessService> _access = new();

    private GetStudentProfileQueryHandler CreateHandler() =>
        new(_students.Object, _profiles.Object, _cohorts.Object, _access.Object);

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-31")]
    public async Task HandleAsync_UnassignedStudent_Throws403Async()
    {
        // Arrange (GIVEN) a Professional assigned to no assessment of the student "Luis Anaya" (2)
        _students.Setup(Repository => Repository.GetByIdAsync(2)).ReturnsAsync(new Student { Id = 2, Name = "Luis Anaya" });
        _access
            .Setup(Service => Service.EnsureCanAccessStudentAsync(2, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BussinessException("You do not have access to this student.", 403));
        GetStudentProfileQueryHandler handler = CreateHandler();

        // Act (WHEN) they request the profile of "Luis Anaya"
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new GetStudentProfileQuery(2), CancellationToken.None));

        // Assert (THEN) 403 and no data of "Luis Anaya" is read
        Assert.Equal(403, error.StatusCode);
        _profiles.Verify(Repository => Repository.GetByStudentIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_AssignedStudent_ReturnsTheProfileAsync()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(1)).ReturnsAsync(new Student { Id = 1, Name = "Ana Torres", Email = "ana@jala.test" });
        _profiles.Setup(Repository => Repository.GetByStudentIdAsync(1)).ReturnsAsync((StudentProfile?)null);
        GetStudentProfileQueryHandler handler = CreateHandler();

        StudentRegistrationDto result = await handler.Handle(new GetStudentProfileQuery(1), CancellationToken.None);

        Assert.Equal(1, result.StudentId);
        _access.Verify(Service => Service.EnsureCanAccessStudentAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UnknownStudent_Throws404BeforeCheckingAccessAsync()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(99)).ReturnsAsync((Student?)null);
        GetStudentProfileQueryHandler handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetStudentProfileQuery(99), CancellationToken.None));

        _access.Verify(Service => Service.EnsureCanAccessStudentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
