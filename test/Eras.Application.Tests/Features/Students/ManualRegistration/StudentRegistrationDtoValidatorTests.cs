using Eras.Application.DTOs.Student;
using Eras.Application.Validation;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

public class StudentRegistrationDtoValidatorTests
{
    private readonly StudentRegistrationDtoValidator _validator = new();

    [Fact]
    public void Validate_MinimalValidDto_Passes()
    {
        Assert.True(_validator.Validate(StudentRegistrationTestData.ValidDto()).IsValid);
    }

    [Fact]
    public void Validate_FullyPopulatedDto_Passes()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.DateOfBirth = new DateOnly(2001, 5, 17);
        dto.Gender = "Female";
        dto.Nationality = "Bolivian";
        dto.CountryOfBirth = "Bolivia";
        dto.SecondaryEmail = "ana.alt@gmail.com";
        dto.MobileNumber = "+591 (700) 123-456";
        dto.Country = "Bolivia";
        dto.StateProvince = "Cochabamba";
        dto.City = "Cochabamba";
        dto.Street = "Av. América 123";
        dto.PostalCode = "0000";
        dto.LevelOfStudy = "Bachelor";
        dto.FacultySchool = "Engineering";
        dto.StudyModality = "Online";
        dto.PreviousInstitution = "Colegio San Agustín";

        Assert.True(_validator.Validate(dto).IsValid);
    }

    [Theory]
    [InlineData(nameof(StudentRegistrationDto.FirstName))]
    [InlineData(nameof(StudentRegistrationDto.LastName))]
    [InlineData(nameof(StudentRegistrationDto.IdPassportNumber))]
    [InlineData(nameof(StudentRegistrationDto.PrimaryEmail))]
    public void Validate_MissingMandatoryField_Fails(string property)
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        typeof(StudentRegistrationDto).GetProperty(property)!.SetValue(dto, "  ");

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, Error => Error.PropertyName == property);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@b")]
    [InlineData("a b@c.com")]
    public void Validate_InvalidPrimaryEmail_Fails(string email)
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.PrimaryEmail = email;

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_SecondaryEmailEqualToPrimary_Fails()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.SecondaryEmail = dto.PrimaryEmail.ToUpperInvariant();

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, Error => Error.ErrorMessage.Contains("different"));
    }

    [Theory]
    [InlineData("Ana123")]
    [InlineData("Ana_")]
    [InlineData("<script>")]
    public void Validate_NamesWithInvalidCharacters_Fail(string name)
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.FirstName = name;

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_DateOfBirthInTheFuture_Fails()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_DateOfBirthBefore1900_Fails()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.DateOfBirth = new DateOnly(1899, 12, 31);

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Theory]
    [InlineData("12")]
    [InlineData("abc12345")]
    [InlineData("+1234567890123456789012")]
    public void Validate_InvalidMobileNumber_Fails(string phone)
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.MobileNumber = phone;

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Validate_TooLongFields_Fail()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.City = new string('x', 101);

        Assert.False(_validator.Validate(dto).IsValid);
    }
}
