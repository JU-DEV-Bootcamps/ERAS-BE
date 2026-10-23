using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class StudentDTOValidationTests
{
    private static StudentDTO CreateValidDTO() => new StudentDTO
    {
        Id = 1,
        Uuid = "f918fe93-76db-440f-bf8e-5f68d42246d4",
        Name = "Jhon Doe",
        Email = "jhon.doe@mail.com"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        StudentDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Id_Negative_Fails(int InvalidId)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Id = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Student Id must be zero or greater.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Id), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Uuid_Required_FailsWhenMissing(string InvalidUuid)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("UUID is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(UuidFormatTestData))]
    public void Uuid_Fails_WhenRegExpDoesNotMatch(string InvalidUuid)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("UUID format is invalid.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Student name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        StudentDTO dto = CreateValidDTO();
        dto.Name = new string('a', 1);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 2 and 254 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        StudentDTO dto = CreateValidDTO();
        dto.Name = new string('a', 255);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 2 and 254 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Juan.Perez")]
    [InlineData("Zeus 2")]
    [InlineData("C|audio")]
    public void Name_Fails_WhenRegExpDoesNotMatch(string InvalidName)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name can only contain letters and spaces.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Email_Required_FailsWhenMissing(string InvalidEmail)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Email = InvalidEmail;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Email is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Email), results.First().MemberNames);
    }

    [Fact]
    public void Email_Fails_WhenLengthTooLong()
    {
        StudentDTO dto = CreateValidDTO();
        dto.Email = new string('a', 255) + new string("@mail.com");

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Email must be less than 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Email), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(EmailFormatTestData))]
    public void Email_Fails_WhenRegExpDoesNotMatch(string InvalidEmail)
    {
        StudentDTO dto = CreateValidDTO();
        dto.Email = InvalidEmail;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Email format is invalid.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDTO.Email), results.First().MemberNames);
    }
}