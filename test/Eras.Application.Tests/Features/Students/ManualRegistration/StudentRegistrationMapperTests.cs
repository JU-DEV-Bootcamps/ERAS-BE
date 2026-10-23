using Eras.Application.DTOs.Student;
using Eras.Application.Mappers;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

public class StudentRegistrationMapperTests
{
    [Fact]
    public void ComposeFullName_JoinsNonEmptyPartsWithSingleSpaces()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.FirstName = " Ana ";
        dto.MiddleName = "   ";
        dto.LastName = "Pérez";

        Assert.Equal("Ana Pérez", dto.ComposeFullName());
    }

    [Fact]
    public void ComposeFullName_IncludesMiddleName()
    {
        Assert.Equal("Ana María Pérez", StudentRegistrationTestData.ValidDto().ComposeFullName());
    }

    [Fact]
    public void NormalizeIdPassport_TrimsAndUppercases()
    {
        Assert.Equal("AB-123456", StudentRegistrationMapper.NormalizeIdPassport("  ab-123456 "));
    }

    [Fact]
    public void ApplyTo_TrimsValuesAndNullsBlankOptionals()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.FirstName = "  Ana ";
        dto.Gender = "   ";
        dto.City = " Cochabamba ";
        StudentProfile profile = new();

        dto.ApplyTo(profile);

        Assert.Equal("Ana", profile.FirstName);
        Assert.Null(profile.Gender);
        Assert.Equal("Cochabamba", profile.City);
        Assert.Equal("AB-123456", profile.IdPassportNumber);
    }

    [Fact]
    public void ApplyTo_WithoutProfileViewFields_LeavesThemNull()
    {
        StudentProfile profile = new();

        StudentRegistrationTestData.ValidDto().ApplyTo(profile);

        Assert.Null(profile.SisId);
        Assert.Null(profile.Cgpa);
        Assert.Null(profile.SapStatus);
        Assert.Null(profile.IsCurrentCourseThirdAttempt);
        Assert.Null(profile.PassedCoursesCount);
        Assert.Null(profile.FailedCoursesCount);
        Assert.Null(profile.CurrentCourses);
        Assert.Null(profile.IdentifiedRisks);
        Assert.Null(profile.SupportReceived);
    }

    [Fact]
    public void ApplyTo_CopiesAndCleansProfileViewFields()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.SisId = " STU-001 ";
        dto.Cgpa = 3.45m;
        dto.SapStatus = "  ";
        dto.IsCurrentCourseThirdAttempt = true;
        dto.PassedCoursesCount = 12;
        dto.FailedCoursesCount = 0;
        dto.CurrentCourses =
        [
            new StudentCurrentCourseDto { Name = " Algorithms ", Group = " G1 ", Section = " " },
            new StudentCurrentCourseDto { Name = "  " },
        ];
        dto.IdentifiedRisks = ["Low attendance", " ", " Late payments "];
        dto.SupportReceived = [];
        StudentProfile profile = new();

        dto.ApplyTo(profile);

        Assert.Equal("STU-001", profile.SisId);
        Assert.Equal(3.45m, profile.Cgpa);
        Assert.Null(profile.SapStatus);
        Assert.True(profile.IsCurrentCourseThirdAttempt);
        Assert.Equal(12, profile.PassedCoursesCount);
        Assert.Equal(0, profile.FailedCoursesCount);
        StudentCurrentCourse course = Assert.Single(profile.CurrentCourses!);
        Assert.Equal("Algorithms", course.Name);
        Assert.Equal("G1", course.Group);
        Assert.Null(course.Section);
        Assert.Equal(["Low attendance", "Late payments"], profile.IdentifiedRisks);
        Assert.Empty(profile.SupportReceived!);
    }

    [Fact]
    public void ToDto_MapsProfileViewFields()
    {
        Student student = new() { Id = 9, Email = "ana@jala.university" };
        StudentProfile profile = new()
        {
            FirstName = "Ana",
            LastName = "Pérez",
            IdPassportNumber = "X1",
            SisId = "STU-001",
            Cgpa = 3.45m,
            SapStatus = "Satisfactory",
            IsCurrentCourseThirdAttempt = false,
            PassedCoursesCount = 12,
            FailedCoursesCount = 1,
            CurrentCourses = [new StudentCurrentCourse { Name = "Algorithms", Group = "G1", Section = "A" }],
            IdentifiedRisks = ["Low attendance"],
            SupportReceived = ["Tutoring"],
        };

        StudentRegistrationDto dto = profile.ToDto(student);

        Assert.Equal("STU-001", dto.SisId);
        Assert.Equal(3.45m, dto.Cgpa);
        Assert.Equal("Satisfactory", dto.SapStatus);
        Assert.False(dto.IsCurrentCourseThirdAttempt);
        Assert.Equal(12, dto.PassedCoursesCount);
        Assert.Equal(1, dto.FailedCoursesCount);
        StudentCurrentCourseDto course = Assert.Single(dto.CurrentCourses!);
        Assert.Equal(("Algorithms", "G1", "A"), (course.Name, course.Group, course.Section));
        Assert.Equal(["Low attendance"], dto.IdentifiedRisks);
        Assert.Equal(["Tutoring"], dto.SupportReceived);
    }

    [Fact]
    public void ToDto_WithoutProfileViewFields_ReturnsNulls()
    {
        StudentRegistrationDto dto = new StudentProfile { FirstName = "Ana", LastName = "Pérez", IdPassportNumber = "X1" }
            .ToDto(new Student { Id = 9, Email = "ana@jala.university" });

        Assert.Null(dto.SisId);
        Assert.Null(dto.Cgpa);
        Assert.Null(dto.CurrentCourses);
        Assert.Null(dto.IdentifiedRisks);
        Assert.Null(dto.SupportReceived);
    }

    [Fact]
    public void ToDto_IncludesGivenCohortAndNullWhenOmitted()
    {
        Student student = new() { Id = 9, Email = "ana@jala.university" };
        StudentProfile profile = new() { FirstName = "Ana", LastName = "Pérez", IdPassportNumber = "X1" };

        Assert.Equal(4, profile.ToDto(student, 4).CohortId);
        Assert.Null(profile.ToDto(student).CohortId);
    }

    [Fact]
    public void ToDto_UsesStudentEmailAndImportedFlag()
    {
        Student student = new() { Id = 9, Email = "ana@jala.university", IsImported = true };
        StudentProfile profile = new() { FirstName = "Ana", LastName = "Pérez", IdPassportNumber = "X1" };

        StudentRegistrationDto dto = profile.ToDto(student);

        Assert.Equal(9, dto.StudentId);
        Assert.Equal("ana@jala.university", dto.PrimaryEmail);
        Assert.True(dto.IsImported);
        Assert.Equal("X1", dto.IdPassportNumber);
    }
}
