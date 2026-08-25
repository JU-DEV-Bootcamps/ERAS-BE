namespace Eras.Application.Contracts.Services;

/// <summary>Per-intervention mismatch between the legacy array length and the migrated `Attachment` row count (US 1.6, Task 1.6.1's count-validation AC).</summary>
public sealed record InterventionAttachmentMigrationValidationFailure(
    int InterventionId,
    int ExpectedCount,
    int ActualCount);

public sealed record InterventionAttachmentMigrationResult
{
    public int InterventionsProcessed { get; init; }
    public int AttachmentsCreated { get; init; }
    public IReadOnlyCollection<int> InterventionsSkippedDueToMismatchedArrays { get; init; } = [];
    public IReadOnlyCollection<InterventionAttachmentMigrationValidationFailure> ValidationFailures { get; init; } = [];

    public bool IsValid =>
        ValidationFailures.Count == 0 && InterventionsSkippedDueToMismatchedArrays.Count == 0;
}
