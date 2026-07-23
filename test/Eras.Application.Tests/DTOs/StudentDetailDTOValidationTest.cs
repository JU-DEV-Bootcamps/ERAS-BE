using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class StudentDetailDTOValidationTests
{
    private static StudentDetailDTO CreateValidDTO() => new StudentDetailDTO
    {
        StudentId = 1
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        StudentDetailDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    public void StudentId_Fail_WhenSetToZero(int InvalidId)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.StudentId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Student Id must be greater than 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.StudentId), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(32768)]
    public void EnrolledCourses_Fail_WithValuesOutOfRange(int InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.EnrolledCourses = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Enrolled courses must be greater than 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.EnrolledCourses), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(32768)]
    public void GradedCourses_Fail_WithValuesOutOfRange(int InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.GradedCourses = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Graded courses must be between 0 and 32767.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.GradedCourses), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(32768)]
    public void TimeDeliveryRate_Fail_WithValuesOutOfRange(int InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.TimeDeliveryRate = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Time delivery rate must be between 0 and 32767.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.TimeDeliveryRate), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(18.01)]
    public void AvgScore_Fail_WithValuesOutOfRange(decimal InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.AvgScore = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Average score must be between 0 and 18.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.AvgScore), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(18.01)]
    public void CoursesUnderAvg_Fail_WithValuesOutOfRange(decimal InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.CoursesUnderAvg = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Courses under average must be between 0 and 18.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.CoursesUnderAvg), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(18.01)]
    public void PureScoreDiff_Fail_WithValuesOutOfRange(decimal InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.PureScoreDiff = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Pure score difference must be between 0 and 18.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.PureScoreDiff), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(18.01)]
    public void StandardScoreDiff_Fail_WithValuesOutOfRange(decimal InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.StandardScoreDiff = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Standard score difference must be between 0 and 18.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.StandardScoreDiff), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(32768)]
    public void LastAccessDays_Fail_WithValuesOutOfRange(int InvalidValue)
    {
        StudentDetailDTO dto = CreateValidDTO();
        dto.LastAccessDays = InvalidValue;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Last access days must be between 0 and 32767.", results.First().ErrorMessage);
        Assert.Contains(nameof(StudentDetailDTO.LastAccessDays), results.First().MemberNames);
    }
}