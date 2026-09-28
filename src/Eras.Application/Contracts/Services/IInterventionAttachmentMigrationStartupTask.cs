namespace Eras.Application.Contracts.Services;

/// <summary>
/// Boot-time orchestration for the Intervention attachment data migration —
/// separate from <see cref="IInterventionAttachmentMigrationService"/>, which only knows how to
/// run the migration itself. This owns the surrounding policy: whether it needs to run at all
/// (via <c>IDataMigrationCompletionRepository</c>), what to log, and when to mark it complete.
/// Kept apart so each half stays independently testable and <c>Program.cs</c> only needs a single
/// call to trigger the whole thing.
/// </summary>
public interface IInterventionAttachmentMigrationStartupTask
{
    Task RunAsync(CancellationToken CancellationToken = default);
}
