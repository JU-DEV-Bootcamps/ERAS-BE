using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class CohortDTOValidationTests
{
    private static CohortDTO CreateValidDTO() => new CohortDTO
    {
        Name = "Cohort 1 (2026-1)",
        CourseCode = "ING-101"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        CohortDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        CohortDTO dto = CreateValidDTO();
        dto.Name = InvalidName!;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cohort name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        CohortDTO dto = CreateValidDTO();
        dto.Name = new string('a', 51);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cohort name must be between 3 and 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        CohortDTO dto = CreateValidDTO();
        dto.Name = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cohort name must be between 3 and 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Cohort #125")]
    [InlineData("Cohort2?")]
    [InlineData("Cohort_2026")]
    [InlineData("Cohort_2026*")]
    public void Name_Fails_WhenDoesNotMatch_RegExp(string InvalidName)
    {
        CohortDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cohort name can only contain letters, numbers, spaces, and hyphens, and parentheses.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void CourseCode_Required_FailsWhenMissing(string InvalidCourseCode)
    {
        CohortDTO dto = CreateValidDTO();
        dto.CourseCode = InvalidCourseCode!;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Course code is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.CourseCode), results.First().MemberNames);
    }

    [Fact]
    public void CourseCode_Fails_WhenLengthTooLong()
    {
        CohortDTO dto = CreateValidDTO();
        dto.CourseCode = new string('A', 51);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Course code must be less than 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.CourseCode), results.First().MemberNames);
    }

    [Theory]
    [InlineData("ing-123")]
    [InlineData("ING_123")]
    [InlineData("ESD123!")]
    [InlineData("ESD*123")]
    public void CourseCode_Fails_WhenDoesNotMatch_RegExp(string InvalidCourseCode)
    {
        CohortDTO dto = CreateValidDTO();
        dto.CourseCode = InvalidCourseCode;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Course code can only contain uppercase letters, numbers, and hyphens.", results.First().ErrorMessage);
        Assert.Contains(nameof(CohortDTO.CourseCode), results.First().MemberNames);
    }
}