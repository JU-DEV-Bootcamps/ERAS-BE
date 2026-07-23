using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class VariableDTOValidationTests
{
    private static VariableDTO CreateValidDTO() => new VariableDTO
    {
        Name = "Test variable",
        Position = 1,
        Type = "multipleChoice"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        VariableDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidId)
    {
        VariableDTO dto = CreateValidDTO();
        dto.Name = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Variable name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        VariableDTO dto = CreateValidDTO();
        dto.Name = new string('a', 251);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Variable name must be between 2 and 250 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        VariableDTO dto = CreateValidDTO();
        dto.Name = new string('a', 1);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Variable name must be between 2 and 250 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(SqlInjectionTestData))]
    public void Name_Fails_WithSqlInjectionPattern(string InjectionPattern)
    {
        VariableDTO dto = CreateValidDTO();
        dto.Name = InjectionPattern;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal($"The field {nameof(VariableDTO.Name)} contains potentially unsafe content that could be used for SQL injection attacks.", results.First().ErrorMessage);
    }

    [Theory]
    [InlineData(-1)]
    public void Position_Negative_FailsRange(int InvalidPosition)
    {
        VariableDTO dto = CreateValidDTO();
        dto.Position = InvalidPosition;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Position must be zero or a positive integer.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Position), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Type_Required_FailsWhenMissing(string InvalidType)
    {
        VariableDTO dto = CreateValidDTO();
        dto.Type = InvalidType;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Type is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Type), results.First().MemberNames);
    }

    [Fact]
    public void Type_Fails_WhenLengthTooLong()
    {
        VariableDTO dto = CreateValidDTO();
        dto.Type = new string('a', 51);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Type must be between 3 and 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Type), results.First().MemberNames);
    }

    [Fact]
    public void Type_Fails_WhenLengthTooShort()
    {
        VariableDTO dto = CreateValidDTO();
        dto.Type = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Type must be between 3 and 50 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Type), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Type 1")]
    [InlineData("type#238")]
    public void Type_Fails_WhenRegExpDoesNotMatch(string InvalidType)
    {
        VariableDTO dto = CreateValidDTO();
        dto.Type = InvalidType;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Type can only contain letters, numbers, dashes, and underscores.", results.First().ErrorMessage);
        Assert.Contains(nameof(VariableDTO.Type), results.First().MemberNames);
    }
}