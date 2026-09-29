namespace Eras.Application.Utils
{
    /// <summary>
    /// Single source of truth for the entity-agnostic storage folder prefix
    /// (`{entityType}/{entityId}`) that callers of <c>IFileStorageService.SaveAsync</c>'s
    /// `folder` argument build from. Combined with the physical file name the provider itself
    /// generates (`{uuid}.{ext}`), this produces the full `{entityType}/{entityId}/{uuid}.{ext}`
    /// key scheme — without every caller hand-rolling the string (previously duplicated,
    /// independently, in the upload and delete Intervention attachment handlers).
    /// </summary>
    public static class AttachmentKeyScheme
    {
        public static string BuildFolder(string entityType, int entityId) => $"{entityType}/{entityId}";
    }
}
