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
