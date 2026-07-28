using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class JUServiceDTOValidationTests
{
    private static JUServiceDTO CreateValidDTO() => new JUServiceDTO
    {
        Id = 1,
        Name = "Test Service",
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        JUServiceDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Id_Negative_FailsRange(int InvalidId)
    {
        JUServiceDTO dto = CreateValidDTO();
        dto.Id = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Id must be greater or equal to 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUServiceDTO.Id), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        JUServiceDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("JUService name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUServiceDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        JUServiceDTO dto = CreateValidDTO();
        dto.Name = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("JUService name must be between 3 and 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUServiceDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        JUServiceDTO dto = CreateValidDTO();
        dto.Name = new string('a', 256);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("JUService name must be between 3 and 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUServiceDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [InlineData("New Service!")]
    [InlineData("Service #2")]
    [InlineData("video|audio")]
    public void Name_Fails_WhenRegExpDoesNotMatch(string InvalidName)
    {
        JUServiceDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("JUService name can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUServiceDTO.Name), results.First().MemberNames);
    }
}