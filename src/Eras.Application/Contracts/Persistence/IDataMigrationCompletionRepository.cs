namespace Eras.Application.Contracts.Persistence;

/// <summary>
/// Tracks which one-off application-level data migrations (see <c>DataMigrationCompletion</c>)
/// have already run to completion, so a migration wired into the app's normal startup — like
/// <c>InterventionAttachmentMigrationService</c>
/// </summary>
public interface IDataMigrationCompletionRepository
{
    Task<bool> IsCompletedAsync(string Name);

    /// <summary>
    /// Records that the named migration finished successfully. Callers must only invoke this after
    /// their own validation confirms the migration is genuinely, fully done — never for a partial
    /// or failed run, or the un-migrated remainder would be silently skipped forever on later boots.
    /// </summary>
    Task MarkCompletedAsync(string Name);
}
