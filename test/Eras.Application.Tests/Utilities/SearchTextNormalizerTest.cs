using Eras.Application.Utils;

namespace Eras.Application.Tests.Utilities;

public class SearchTextNormalizerTest
{
    [Theory]
    [InlineData("José", "jose")]
    [InlineData("MARÍA Ñúñez", "maria nunez")]
    [InlineData("  Ana  ", "ana")]
    [InlineData("_100%", "_100%")]
    public void Normalize_Text_RemovesAccentsCaseAndOuterSpaces(string text, string expected)
    {
        Assert.Equal(expected, SearchTextNormalizer.Normalize(text));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_NullOrBlank_ReturnsEmpty(string? text)
    {
        Assert.Equal(string.Empty, SearchTextNormalizer.Normalize(text));
    }
}
