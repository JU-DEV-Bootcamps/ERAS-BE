using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;

public class PollInstanceDTOValidationTests
{
    private static PollInstanceDTO CreateValidDTO() => new PollInstanceDTO
    {
        Uuid = "f918fe93-76db-440f-bf8e-5f68d42246d4",
        LastVersion = 1,
        LastVersionDate = DateTime.Now
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        PollInstanceDTO dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Uuid_Required_FailsWhenMissing(string InvalidUuid)
    {
        PollInstanceDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);
    
        Assert.Single(results);
        Assert.Equal("UUID is required.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollInstanceDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [ClassData(typeof(UuidFormatTestData))]
    public void Uuid_Fails_WhenRegExpDoesNotMatch(string InvalidUuid)
    {
        PollInstanceDTO dto = CreateValidDTO();
        dto.Uuid = InvalidUuid;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("UUID format is invalid.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollInstanceDTO.Uuid), results.First().MemberNames);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void LastVersion_ZeroOrNegative_Fails(int InvalidVersion)
    {
        PollInstanceDTO dto = CreateValidDTO();
        dto.LastVersion = InvalidVersion;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Last version must be at least 1.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollInstanceDTO.LastVersion), results.First().MemberNames);
    }

    [Fact]
    public void AnswersHash_Fails_WhenLengthTooLong()
    {
        PollInstanceDTO dto = CreateValidDTO();
        dto.AnswersHash = new string('a', 65);

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Answers hash must be 64 characters long.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollInstanceDTO.AnswersHash), results.First().MemberNames);
    }
}