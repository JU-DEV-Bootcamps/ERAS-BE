using System.ComponentModel.DataAnnotations;
using Eras.Application.DTOs;

namespace Eras.Application.Tests.DTOs;

public class JUProfessionalDTOValidationTests
{
    private static JUProfessionalDTO CreateValidDTO() => new JUProfessionalDTO
    {
        Id = 1,
        Name = "Test Evaluation",
        Uuid = "8cd01d9a-48fc-4b4a-95de-03eaa0916f25"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        JUProfessionalDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Id_Negative_FailsRange(int InvalidId)
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Id = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Id must be greater or equal to 0.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Id), results.First().MemberNames);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Name_Required_FailsWhenMissing(string InvalidName)
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Professional name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Name = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Professional name must be between 3 and 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Name = new string('a', 256);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Professional name must be between 3 and 255 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Dr. Strangelove!")]
    [InlineData("Zeus #2")]
    [InlineData("C|audio")]
    public void Name_Fails_WhenRegExpDoesNotMatch(string InvalidName)
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Name = InvalidName;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Professional name can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Uuid_Required_FailsWhenMissing(string InvalidUUID)
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUUID;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Professional UUID is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Uuid), results.First().MemberNames);
    }

    [Fact]
    public void Uuid_Fails_WhenLengthTooShort()
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Uuid = new string('a', 35);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Equal(2, results.Count);
        Assert.Equal("Professional UUID must be exactly 36 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Uuid), results.First().MemberNames);
    }

    [Fact]
    public void Uuid_Fails_WhenLengthTooLong()
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Uuid = new string('a', 37);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Equal(2, results.Count);
        Assert.Equal("Professional UUID must be exactly 36 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [InlineData("7gd01d8z-37fh-4b4a-9537-03epa0916f25")]
    [InlineData("7cd01d8a_37fc_4b4a_9537_03eaa0916f25")]
    public void Uuid_Fails_WhenRegExpDoesNotMatch(string InvalidUuid)
    {
        JUProfessionalDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Professional must follow a valid GUID format.", results.First().ErrorMessage);
        Assert.Contains(nameof(JUProfessionalDTO.Uuid), results.First().MemberNames);
    }
}