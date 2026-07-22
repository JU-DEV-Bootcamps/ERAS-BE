using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;

namespace Eras.Application.Tests.DTOs;
public class ComponentDTOValidationTest
{
    private static ComponentDTO CreateValidDTO() => new ComponentDTO
    {
        Name = "Academic"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        ComponentDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        ComponentDTO dto = CreateValidDTO();
        dto.Name = InvalidName!;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Component name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(ComponentDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        ComponentDTO dto = CreateValidDTO();
        dto.Name = new string('a', 31);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Component name must be between 3 and 30 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ComponentDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        ComponentDTO dto = CreateValidDTO();
        dto.Name = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Component name must be between 3 and 30 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(ComponentDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WithSqlInjectionPattern()
    {
        ComponentDTO dto = CreateValidDTO();
        dto.Name = "'; DROP TABLE Students; --";

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal($"The field {nameof(ComponentDTO.Name)} contains potentially unsafe content that could be used for SQL injection attacks.", results.First().ErrorMessage);
    }
}