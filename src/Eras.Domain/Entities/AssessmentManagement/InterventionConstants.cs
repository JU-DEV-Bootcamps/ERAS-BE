namespace Eras.Domain.Entities.AssessmentManagement;

public static class InterventionConstants
{
    public const int MaxAttachments = 5;

    /// <summary>
    /// The `entityType` value Interventions use in the generic attachment storage key scheme
    /// (`{entityType}/{entityId}/{uuid}.{ext}`, see <c>AttachmentKeyScheme</c>). Kept as the
    /// pre-existing literal ("interventions", lowercase/plural) rather than renamed to match
    /// <c>Attachment.EntityType</c>'s convention elsewhere, so already-uploaded files on disk
    /// stay resolvable at their existing paths.
    /// </summary>
    public const string AttachmentEntityType = "interventions";
}