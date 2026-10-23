using Eras.Domain.Common;

namespace Eras.Domain.Entities;

/// <summary>
/// Marks a one-off application-level data migration (as opposed to an EF Core schema migration,
/// tracked separately by EF itself in the `__EFMigrationsHistory` table) as fully, successfully
/// completed — so it isn't re-run in full on every subsequent application boot.
///
/// A row for a given <see cref="Name"/> must only ever be written after that migration's own
/// validation confirms it succeeded end-to-end. A migration that fails or is interrupted partway
/// must NOT write this row, so the next boot still resumes and re-validates it, rather than
/// silently skipping the un-migrated remainder forever.
/// </summary>
public class DataMigrationCompletion : BaseEntity
{
    /// <summary>Unique name identifying which one-off migration this row marks as complete.</summary>
    public required string Name { get; init; }

    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
}
