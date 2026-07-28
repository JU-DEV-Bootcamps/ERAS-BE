using System.ComponentModel.DataAnnotations;

using Eras.Application.Dtos;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;
public class PollDTOValidationTests
{
    private static PollDTO CreateValidDTO() => new PollDTO
    {
        IdCosmicLatte = "CL-1",
        Uuid = "f918fe93-76db-440f-bf8e-5f68d42246d4",
        Name = "Test Poll",
        FinishedAt = DateTime.Now,
        LastVersion = 1,
        Components = [new Application.DTOs.ComponentDTO()],
        ParentId = "P-ID-1"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        PollDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Id_Negative_FailsRange(int InvalidId)
    {
        PollDTO dto = CreateValidDTO();
        dto.Id = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
        
        Assert.Single(results);
        Assert.Equal("Id must be zero or greater.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Id), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void IdCosmicLatte_Required_FailsWhenMissing(string InvalidId)
    {
        PollDTO dto = CreateValidDTO();
        dto.IdCosmicLatte = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Cosmic Latte Id is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.IdCosmicLatte), results.First().MemberNames);
    }

    [Fact]
    public void IdCosmicLatte_Fails_WhenLengthTooLong()
    {
        PollDTO dto = CreateValidDTO();
        dto.IdCosmicLatte = new string('a', 101);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cosmic Latte Id must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.IdCosmicLatte), results.First().MemberNames);
    }

    [Fact]
    public void IdCosmicLatte_Fails_WhenLengthTooShort()
    {
        PollDTO dto = CreateValidDTO();
        dto.IdCosmicLatte = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cosmic Latte Id must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.IdCosmicLatte), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Cosmic Latte ID 1")]
    [InlineData("id#238")]
    public void IdCosmicLatte_Fails_WhenRegExpDoesNotMatch(string InvalidId)
    {
        PollDTO dto = CreateValidDTO();
        dto.IdCosmicLatte = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Cosmic Latte Id can only contain letters, numbers, dashes and underscores.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.IdCosmicLatte), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Uuid_Required_FailsWhenMissing(string InvalidUuid)
    {
        PollDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("UUID is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(UuidFormatTestData))]
    public void Uuid_Fails_WhenRegExpDoesNotMatch(string InvalidUuid)
    {
        PollDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("UUID format is invalid.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Name_Required_FailsWhenMissing(string InvalidId)
    {
        PollDTO dto = CreateValidDTO();
        dto.Name = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Name is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooLong()
    {
        PollDTO dto = CreateValidDTO();
        dto.Name = new string('a', 101);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Name), results.First().MemberNames);
    }

    [Fact]
    public void Name_Fails_WhenLengthTooShort()
    {
        PollDTO dto = CreateValidDTO();
        dto.Name = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Name must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Name), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(SqlInjectionTestData))]
    public void Name_Fails_WithSqlInjectionPattern(string InjectionPattern)
    {
        PollDTO dto = CreateValidDTO();
        dto.Name = InjectionPattern;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal($"The field {nameof(PollDTO.Name)} contains potentially unsafe content that could be used for SQL injection attacks.", results.First().ErrorMessage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void LastVersion_ZeroOrNegative_Fails(int InvalidVersion)
    {
        PollDTO dto = CreateValidDTO();
        dto.LastVersion = InvalidVersion;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Last version must be at least 1.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.LastVersion), results.First().MemberNames);
    }

    [Fact]
    public void Components_Required_FailsWhenMissing()
    {
        PollDTO dto = CreateValidDTO();
        dto.Components = null;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("At least one component is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Components), results.First().MemberNames);
    }

    [Fact]
    public void Components_Length_FailsWhenEmpty()
    {
        PollDTO dto = CreateValidDTO();
        dto.Components = [];

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("The poll must contain at least one component.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.Components), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void ParentId_Required_FailsWhenMissing(string InvalidId)
    {
        PollDTO dto = CreateValidDTO();
        dto.ParentId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("Parent Id is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.ParentId), results.First().MemberNames);
    }

    [Fact]
    public void ParentId_Fails_WhenLengthTooLong()
    {
        PollDTO dto = CreateValidDTO();
        dto.ParentId = new string('a', 101);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Parent Id must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.ParentId), results.First().MemberNames);
    }

    [Fact]
    public void ParentId_Fails_WhenLengthTooShort()
    {
        PollDTO dto = CreateValidDTO();
        dto.ParentId = new string('a', 2);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Parent Id must be between 3 and 100 characters.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.ParentId), results.First().MemberNames);
    }

    [Theory]
    [InlineData("Parent ID 1")]
    [InlineData("id#238")]
    public void ParentId_Fails_WhenRegExpDoesNotMatch(string InvalidId)
    {
        PollDTO dto = CreateValidDTO();
        dto.ParentId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Parent Id can only contain letters, numbers, dashes and underscores.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollDTO.ParentId), results.First().MemberNames);
    }
}