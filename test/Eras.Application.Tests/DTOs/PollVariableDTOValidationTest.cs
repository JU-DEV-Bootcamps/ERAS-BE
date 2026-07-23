using System.ComponentModel.DataAnnotations;

using Eras.Application.DTOs;
using Eras.Application.DTOs.Poll;
using Eras.Application.Tests.TestUtils;

namespace Eras.Application.Tests.DTOs;

public class PollVariableDTOValidationTests
{
    private static PollVariableDto CreateValidDTO() => new PollVariableDto
    {
        PollId = 1,
        VariableId = 1,
        VariableName = "Test variable"
    };

    [Fact]
    public void ValidDTO_PassesValidation()
    {
        PollVariableDto dto = CreateValidDTO();

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PollId_ZeroOrNegative_Fails(int InvalidId)
    {
        PollVariableDto dto = CreateValidDTO();
        dto.PollId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Poll Id must be greater or equal to 1.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollVariableDto.PollId), results.First().MemberNames);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void VariableId_ZeroOrNegative_Fails(int InvalidId)
    {
        PollVariableDto dto = CreateValidDTO();
        dto.VariableId = InvalidId;

        IList<ValidationResult> results = ValidationTestHelper.Validate(dto);

        Assert.Single(results);
        Assert.Equal("Variable Id must be greater or equal to 1.", results.First().ErrorMessage);
        Assert.Contains(nameof(PollVariableDto.VariableId), results.First().MemberNames);
    }
}