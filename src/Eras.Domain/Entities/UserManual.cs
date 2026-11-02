namespace Eras.Domain.Entities;

public enum UserManualFormat
{
    Pdf,
    Odt,
}

/// <summary>A user manual declared in the repository's manuals manifest.</summary>
public sealed record UserManual(
    string Id,
    string Title,
    string Language,
    string Version,
    string FileName,
    UserManualFormat Format);
