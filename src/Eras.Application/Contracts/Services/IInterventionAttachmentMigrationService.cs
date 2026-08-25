namespace Eras.Application.Contracts.Services;

/// <summary>
/// One-time data migration: backfills `Attachment` rows from every
/// Intervention's legacy `Attachments`/`AttachmentHashes` `text[]` pair.
/// This is data migration, not schema migration, and shouldn't run as a side effect of app boot.
/// </summary>
public interface IInterventionAttachmentMigrationService
{
    /// <summary>
    /// Idempotent and safe to re-run: an existing `Attachment` row with a matching
    /// `(entity_type, entity_id, content_hash)` is treated as already migrated and skipped, so a
    /// partial or repeated run never creates duplicates.
    /// </summary>
    Task<InterventionAttachmentMigrationResult> MigrateAsync(CancellationToken CancellationToken = default);
}
