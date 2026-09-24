using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Services;

/// <summary>
/// See <see cref="IInterventionAttachmentMigrationStartupTask"/>. Kept out of <c>Program.cs</c>
/// so this orchestration is independently unit-testable.
/// </summary>
public sealed class InterventionAttachmentMigrationStartupTask(
    IInterventionAttachmentMigrationService MigrationService,
    IDataMigrationCompletionRepository CompletionRepository,
    ILogger<InterventionAttachmentMigrationStartupTask> Logger) : IInterventionAttachmentMigrationStartupTask
{
    public async Task RunAsync(CancellationToken CancellationToken = default)
    {
        // Skip once a prior run finished fully valid. Not gated on existing rows instead, since a
        // crash mid-run would leave some rows without the migration being done — the marker is
        // only set when IsValid, so an interrupted run still resumes next boot.
        if (await CompletionRepository.IsCompletedAsync(InterventionAttachmentMigrationService.MigrationName))
        {
            Logger.LogDebug("Intervention attachment migration already completed — skipping.");
            return;
        }

        InterventionAttachmentMigrationResult result = await MigrationService.MigrateAsync(CancellationToken);

        if (result.AttachmentsCreated > 0)
            Logger.LogInformation(
                "Intervention attachment migration: {Processed} intervention(s) processed, {Created} attachment(s) created.",
                result.InterventionsProcessed, result.AttachmentsCreated);

        foreach (int interventionId in result.InterventionsSkippedDueToMismatchedArrays)
            Logger.LogError(
                "Intervention {InterventionId}: Attachments/AttachmentHashes length mismatch — needs manual review.",
                interventionId);

        foreach (InterventionAttachmentMigrationValidationFailure failure in result.ValidationFailures)
            Logger.LogError(
                "Intervention {InterventionId}: expected {Expected} attachment(s), found {Actual} after migration — needs manual review.",
                failure.InterventionId, failure.ExpectedCount, failure.ActualCount);

        // Doesn't throw on a validation failure — it's already logged above, and not marking
        // complete means the next boot retries automatically.
        if (result.IsValid)
            await CompletionRepository.MarkCompletedAsync(InterventionAttachmentMigrationService.MigrationName);
    }
}
