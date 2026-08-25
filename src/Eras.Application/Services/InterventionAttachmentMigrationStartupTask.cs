using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Services;

/// <summary>
/// See <see cref="IInterventionAttachmentMigrationStartupTask"/>. Extracted out of
/// <c>Program.cs</c> so that file stays a thin bootstrap and this orchestration — gating on the
/// completion marker, logging, deciding when to mark done — is independently unit-testable.
/// </summary>
public sealed class InterventionAttachmentMigrationStartupTask(
    IInterventionAttachmentMigrationService MigrationService,
    IDataMigrationCompletionRepository CompletionRepository,
    ILogger<InterventionAttachmentMigrationStartupTask> Logger) : IInterventionAttachmentMigrationStartupTask
{
    public async Task RunAsync(CancellationToken CancellationToken = default)
    {
        // Gated on a completion marker, not run unconditionally on every boot: once a run finishes
        // with a fully valid result, later boots skip it entirely instead of re-scanning every
        // Intervention just to find nothing left to do. Deliberately NOT gated on "does the
        // attachments table already have any interventions rows", which would look similar but
        // isn't the same thing — a crash partway through a first migration run would leave some
        // rows present without the migration being done, and a row-count check would then skip the
        // remainder forever. The completion marker is only ever written after MigrateAsync reports
        // IsValid == true, so an interrupted run is still resumed (and re-validated) on the next
        // boot rather than silently abandoned.
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

        // Deliberately does NOT throw/crash startup on a validation failure. Unlike a failed schema
        // migration — which legitimately should stop the app from serving traffic against a
        // half-migrated schema — a data-migration mismatch here doesn't put the app in an unsafe
        // state to run in. It's logged loudly (LogError, above) so it gets noticed and investigated,
        // not treated as a boot-blocking failure that would crash-loop every replica. A failure here
        // also means the completion marker below is correctly never written, so the next boot
        // retries rather than accepting a known-bad result as done.
        if (result.IsValid)
            await CompletionRepository.MarkCompletedAsync(InterventionAttachmentMigrationService.MigrationName);
    }
}
