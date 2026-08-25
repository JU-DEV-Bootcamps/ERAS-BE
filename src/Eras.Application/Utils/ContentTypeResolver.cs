namespace Eras.Application.Utils
{
    /// <summary>
    /// Single source of truth for guessing a MIME type from a file name's extension.
    ///
    /// This is a best-effort guess from the extension alone, not a content inspection — see
    /// <see cref="FileSignatureValidator"/> for actual magic-byte validation.
    /// </summary>
    public static class ContentTypeResolver
    {
        public static string Resolve(string FileName) =>
            Path.GetExtension(FileName).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
    }
}
