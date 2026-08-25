using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Services;

public sealed class InterventionAttachmentMigrationService(
    IAssessmentRepository AssessmentRepository,
    IAttachmentRepository AttachmentRepository,
    ILogger<InterventionAttachmentMigrationService> Logger) : IInterventionAttachmentMigrationService
{
    /// <summary>
    /// Name this migration is tracked under in <c>DataMigrationCompletion</c> — the key
    /// <c>Program.cs</c> checks via <c>IDataMigrationCompletionRepository</c> before deciding
    /// whether to run <see cref="MigrateAsync"/> again on a given boot.
    /// </summary>
    public const string MigrationName = "intervention-attachment-migration";

    /// <summary>
    /// Real uploader identity was never captured for these files
    /// </summary>
    private const string MigrationCreatedBy = "legacy-migration";

    public async Task<InterventionAttachmentMigrationResult> MigrateAsync(CancellationToken CancellationToken = default)
    {
        IEnumerable<Assessment> assessments = await AssessmentRepository.GetAllAsync();
        List<Intervention> interventions = assessments.SelectMany(Assessment => Assessment.Interventions).ToList();

        int processed = 0;
        int created = 0;
        var mismatchedArrays = new List<int>();
        var validationFailures = new List<InterventionAttachmentMigrationValidationFailure>();

        foreach (Intervention intervention in interventions)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (intervention.Attachments.Count == 0)
                continue;

            processed++;

            // Attachments/AttachmentHashes are parallel arrays (RemoveAttachmentAsync relies on
            // matching indexes) — a length mismatch means the data is already corrupt and needs a
            // human to look at it, not a best-effort guess from this script.
            if (intervention.Attachments.Count != intervention.AttachmentHashes.Count)
            {
                Logger.LogError(
                    "Intervention {InterventionId}: Attachments ({PathCount}) / AttachmentHashes ({HashCount}) length mismatch — skipped, needs manual review.",
                    intervention.Id, intervention.Attachments.Count, intervention.AttachmentHashes.Count);
                mismatchedArrays.Add(intervention.Id);
                continue;
            }

            IEnumerable<Attachment> alreadyMigrated = await AttachmentRepository.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, intervention.Id);
            HashSet<string> alreadyMigratedHashes = alreadyMigrated.Select(Obj=> Obj.ContentHash).ToHashSet();

            List<string> paths = intervention.Attachments.ToList();
            List<string> hashes = intervention.AttachmentHashes.ToList();
            int createdForThisIntervention = 0;

            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                string hash = hashes[i];

                if (alreadyMigratedHashes.Contains(hash))
                    continue; // migrated by a previous run — idempotent, skip rather than duplicate

                // `MimeType` and `OriginalFileName` were never originally captured historically, so
                // both are derived from `path` instead — `OriginalFileName` is really the GUID
                // SaveAsync generated (not the user's true original name), and `MimeType` is guessed
                // from that same path's extension, which SaveAsync did preserve verbatim.
                var attachment = new Attachment
                {
                    EntityType = InterventionConstants.AttachmentEntityType,
                    EntityId = intervention.Id,
                    StorageKey = path,
                    ContentHash = hash,
                    OriginalFileName = Path.GetFileName(path),
                    MimeType = ContentTypeResolver.Resolve(path),
                    SizeBytes = null,
                    CreatedBy = MigrationCreatedBy,
                    CreatedAt = DateTime.UtcNow
                };
                await AttachmentRepository.AddAsync(attachment);

                created++;
                createdForThisIntervention++;
            }
            
            int projectedCount = alreadyMigratedHashes.Count + createdForThisIntervention;
            if (projectedCount != paths.Count)
                validationFailures.Add(new InterventionAttachmentMigrationValidationFailure(intervention.Id, paths.Count, projectedCount));
        }

        return new InterventionAttachmentMigrationResult
        {
            InterventionsProcessed = processed,
            AttachmentsCreated = created,
            InterventionsSkippedDueToMismatchedArrays = mismatchedArrays,
            ValidationFailures = validationFailures
        };
    }
}
