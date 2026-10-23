using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class StudentLightDtoValidationTests
{
    private static StudentLightDto CreateValidDTO() => new StudentLightDto
    {
        Id = 1,
        Name = "Jhon Doe"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        StudentLightDto dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Id_Negative_Fails(int InvalidId)
    {
        StudentLightDto dto = CreateValidDTO();
        dto.Id = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Student Id must be zero or greater.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentLightDto.Id), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        StudentLightDto dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Student name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentLightDto.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        StudentLightDto dto = CreateValidDTO();
        dto.Name = new string('a', 1);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 2 and 254 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentLightDto.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        StudentLightDto dto = CreateValidDTO();
        dto.Name = new string('a', 255);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 2 and 254 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentLightDto.Name), results.First().MemberNames);
    }
}