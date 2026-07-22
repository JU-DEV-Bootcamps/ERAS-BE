using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;

namespace Eras.Application.Tests.DTOs;
public class AnswerDTOValidationTests
{
    private static AnswerDTO CreateValidDTO() => new AnswerDTO
    {
        Answer = "This is a valid answer",
        Score = 80,
        PollInstanceId = 1,
        PollVariableId = 2,
        Version = new Domain.Common.VersionInfo()
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        AnswerDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Answer_Required_FailsWhenMissing(string InvalidAnswer)
    {
        AnswerDTO dto = CreateValidDTO();
        dto.Answer = InvalidAnswer!;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Answer text is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(AnswerDTO.Answer), results.First().MemberNames);
    }

    [Fact]
    public void Answer_Fails_WhenLengthTooLong()
    {
        AnswerDTO dto = CreateValidDTO();
        dto.Answer = new string('a', 501);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Answer must be between 1 and 500 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(AnswerDTO.Answer), results.First().MemberNames);
    }

    [Fact]
    public void Answer_Fails_WithSqlInjectionPattern()
    {
        AnswerDTO dto = CreateValidDTO();
        dto.Answer = "'; DROP TABLE Students; --";

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Console.WriteLine("memnames", results.First().MemberNames);
        Assert.Equal($"The field {nameof(AnswerDTO.Answer)} contains potentially unsafe content that could be used for SQL injection attacks.", results.First().ErrorMessage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Score_OutOfRange_FailsValidation(decimal InvalidScore)
    {
        AnswerDTO dto = CreateValidDTO();
        dto.Score = InvalidScore;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Score must be between 0 and 100.", results.First().ErrorMessage);
        Assert.Contains(nameof(AnswerDTO.Score), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    public void PollInstanceId_Negative_FailsRange(int InvalidId)
    {
        AnswerDTO dto = CreateValidDTO();
        dto.PollInstanceId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Poll Instance Id must be zero or greater.", results.First().ErrorMessage);
        Assert.Contains(nameof(AnswerDTO.PollInstanceId), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    public void PollVariableId_Negative_FailsRange(int InvalidId)
    {
        AnswerDTO dto = CreateValidDTO();
        dto.PollVariableId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Poll Variable Id must be zero or greater.", results.First().ErrorMessage);
        Assert.Contains(nameof(AnswerDTO.PollVariableId), results.First().MemberNames);
    }
}