namespace Eras.Domain.Entities.Referrals;

internal static class DomainNormalization
{
    public static string? ToNullableTrimmed(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static string ToTrimmedOrEmpty(string? value)
        => value?.Trim() ?? string.Empty;
}
