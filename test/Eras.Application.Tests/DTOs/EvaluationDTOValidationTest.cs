using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class EvaluationDTOValidationTests
{
    private static EvaluationDTO CreateValidDTO() => new EvaluationDTO
    {
        Id = 1,
        Name = "Test Evaluation",
        StartDate = DateTime.Now,
        EndDate = DateTime.Now,
        PollName = "Test Poll",
        Country = "COL",
        EvaluationPollId = 1,
        PollId = 1,
        ConfigurationId = 2,
        Status = "Completed"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        EvaluationDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.Name = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 3 and 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.Name = new string('a', 51);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 3 and 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void PollName_Required_FailsWhenMissing(string InvalidPollName)
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.PollName = InvalidPollName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Poll name is required", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.PollName), results.First().MemberNames);
    }

    [Fact]
    public void PollName_Fails_WhenLengthTooLong()
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.PollName = new string('a', 101);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Poll name must be less than 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.PollName), results.First().MemberNames);
    }

    [Fact]
    public void Country_Fails_WhenLengthTooLong()
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.Country = new string('a', 11);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Country must be less than 10 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.Country), results.First().MemberNames);
    }

    [Theory]
    [InlineData(-1)]
    public void ConfigurationId_Negative_FailsRange(int InvalidId)
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.ConfigurationId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Configuration Id must be greater or equal to 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.ConfigurationId), results.First().MemberNames);
    }

    [Fact]
    public void Status_Fails_WhenLengthTooLong()
    {
        EvaluationDTO dto = CreateValidDTO();
        dto.Status = new string('a', 31);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Status must be less than 30 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(EvaluationDTO.Status), results.First().MemberNames);
    }
}