using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Queries.GetStudentProfile;
using Eras.Domain.Entities;

using Moq;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

public class GetStudentProfileSisIdTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly GetStudentProfileQueryHandler _handler;

    public GetStudentProfileSisIdTests()
    {
        Mock<IStudentCohortRepository> studentCohorts = new();
        studentCohorts.Setup(R => R.GetCohortIdByStudentIdAsync(It.IsAny<int>())).ReturnsAsync(3);
        _handler = new GetStudentProfileQueryHandler(
            _students.Object, _profiles.Object, studentCohorts.Object, new Mock<IStudentAccessService>().Object);
    }

    private Task<StudentRegistrationDto> Run(int studentId = 7) =>
        _handler.Handle(new GetStudentProfileQuery(studentId), CancellationToken.None);

    private void GivenStudent(Student student, StudentProfile? profile)
    {
        _students.Setup(R => R.GetByIdAsync(student.Id)).ReturnsAsync(student);
        _profiles.Setup(R => R.GetByStudentIdAsync(student.Id)).ReturnsAsync(profile);
    }

    [Fact]
    [Trait("Change", "641-align-student-modal-fields")]
    [Trait("AC", "641-align-student-modal-fields/AC-6")]
    public async Task Handle_ImportedStudent_ReturnsTheImportedIdentifierAsSisIdAsync()
    {
        // Arrange (GIVEN): an imported student without a profile, and another one whose profile has its own SIS ID
        GivenStudent(new Student { Id = 7, Uuid = "S-2002", Email = "a@b.co", IsImported = true }, null);
        GivenStudent(
            new Student { Id = 8, Uuid = "S-2003", Email = "c@d.co", IsImported = true },
            new StudentProfile { StudentId = 8, SisId = "S-9999" });

        // Act (WHEN)
        StudentRegistrationDto withoutProfile = await Run(7);
        StudentRegistrationDto withProfile = await Run(8);

        // Assert (THEN / AND): the value the import stored wins in both cases
        Assert.Equal("S-2002", withoutProfile.SisId);
        Assert.Equal("S-2003", withProfile.SisId);
    }

    [Fact]
    [Trait("Change", "641-align-student-modal-fields")]
    [Trait("AC", "641-align-student-modal-fields/AC-19")]
    public async Task Handle_ManualStudent_ReturnsTheProfileSisIdOrNoneAsync()
    {
        // Arrange (GIVEN): two students registered by hand, one with a SIS ID in the profile and one without
        string generatedIdentifier = Guid.NewGuid().ToString();
        GivenStudent(
            new Student { Id = 7, Uuid = Guid.NewGuid().ToString(), Email = "a@b.co", IsImported = false },
            new StudentProfile { StudentId = 7, SisId = "S-3003" });
        GivenStudent(
            new Student { Id = 8, Uuid = generatedIdentifier, Email = "c@d.co", IsImported = false },
            new StudentProfile { StudentId = 8 });

        // Act (WHEN)
        StudentRegistrationDto withSisId = await Run(7);
        StudentRegistrationDto withoutSisId = await Run(8);

        // Assert (THEN / AND)
        Assert.Equal("S-3003", withSisId.SisId);
        Assert.Null(withoutSisId.SisId);
        Assert.NotEqual(generatedIdentifier, withoutSisId.SisId);
    }
}
