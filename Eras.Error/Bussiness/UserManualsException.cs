namespace Eras.Error.Bussiness;

/// <summary>The error codes a failed list or download of user manuals can carry.</summary>
public static class UserManualsErrorCodes
{
    public const string ManifestMissing = "manuals_manifest_missing";
    public const string ManifestInvalid = "manuals_manifest_invalid";
    public const string FileMissing = "manuals_file_missing";
    public const string NotFound = "manuals_not_found";
}

/// <summary>
/// A user manuals failure. The friendly message is fixed and never names a path; the detail (manual, file,
/// folder and problem) goes only to the server log.
/// </summary>
public sealed class UserManualsException : ErasException, IErrorCodeException
{
    private readonly string _logMessage;

    public string ErrorCode { get; }

    private UserManualsException(
        string ErrorCode,
        string FriendlyMessage,
        string LogMessage,
        Severity Severity,
        int StatusCode)
        : base(FriendlyMessage, Severity, StatusCode)
    {
        this.ErrorCode = ErrorCode;
        _logMessage = LogMessage;
    }

    public static UserManualsException ManifestMissing(string ManifestName, string Folder) => new(
        UserManualsErrorCodes.ManifestMissing,
        "The list of user manuals is not available.",
        $"The user manuals manifest '{ManifestName}' is missing in the folder '{Folder}'.",
        Severity.ERROR,
        500);

    public static UserManualsException ManifestInvalid(string ManifestName, string Problem) => new(
        UserManualsErrorCodes.ManifestInvalid,
        "The list of user manuals is not valid.",
        $"The user manuals manifest '{ManifestName}' is not valid: {Problem}",
        Severity.ERROR,
        500);

    public static UserManualsException FileMissing(string ManualId, string FileName, string Folder) => new(
        UserManualsErrorCodes.FileMissing,
        "The file of this user manual is not available.",
        $"The file '{FileName}' of the manual '{ManualId}' is missing from the folder '{Folder}'.",
        Severity.ERROR,
        500);

    public static UserManualsException NotFound(string ManualId) => new(
        UserManualsErrorCodes.NotFound,
        "This user manual does not exist.",
        $"No manual is declared with the identifier '{WithoutLineBreaks(ManualId)}'.",
        Severity.WARNING,
        404);

    public override void LogException() => LogMessage(_logMessage);

    /// <summary>The identifier comes from the request, so it must not be able to forge a log line.</summary>
    private static string WithoutLineBreaks(string Value) =>
        Value.Replace("\r", string.Empty, StringComparison.Ordinal).Replace("\n", string.Empty, StringComparison.Ordinal);
}
