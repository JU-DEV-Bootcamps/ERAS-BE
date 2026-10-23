using Eras.Application.Utils;

using Xunit;

namespace Eras.Application.Tests.TestUtils;

public class AnswerRiskValidatorTests
{
    private readonly AnswerRiskValidator _validator = new();

    [Theory]
    [InlineData("Yes")]
    [InlineData("No")]
    [InlineData("Some answer")]
    [InlineData("None of the above")]
    [InlineData("Ningunos")]
    [InlineData("- ")]
    public void IsValidAnswer_ShouldReturnTrue_ForValidAnswers(string answer)
    {
        // Act
        var result = _validator.IsValidAnswer(answer);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValidAnswer_ShouldReturnFalse_WhenAnswerIsNullOrEmpty(string? answer)
    {
        // Act
        var result = _validator.IsValidAnswer(answer);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("-")]
    [InlineData("NONE")]
    [InlineData("none")]
    [InlineData("NoNe")]
    [InlineData("NINGUNO")]
    [InlineData("ninguno")]
    [InlineData("Ninguno")]
    [InlineData("NINGUNA")]
    [InlineData("ninguna")]
    [InlineData("Ninguna")]
    public void IsValidAnswer_ShouldBeCaseInsensitive_ForExcludedAnswers(string answer)
    {
        // Act
        var result = _validator.IsValidAnswer(answer);

        // Assert
        Assert.False(result);
    }
}
