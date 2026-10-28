using System.Globalization;
using System.Text;

namespace Eras.Application.Utils;

public static class SearchTextNormalizer
{
    /// <summary>
    /// Trims the text, removes accents and diacritics and lowercases it, so two texts can be compared
    /// without regard to case or accents. Null becomes an empty string.
    /// </summary>
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        string decomposed = text.Trim().Normalize(NormalizationForm.FormD);
        StringBuilder builder = new(decomposed.Length);
        foreach (char character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }
}
