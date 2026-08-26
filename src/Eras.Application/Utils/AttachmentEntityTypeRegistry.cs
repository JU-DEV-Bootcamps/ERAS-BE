using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Utils
{
    /// <summary>
    /// Whitelist of `entityType` values allowed to own attachments through the generic
    /// <c>AttachmentService</c>. A new entity type must be added here explicitly
    /// before it can upload/list/download/delete attachments.
    /// </summary>
    public static class AttachmentEntityTypeRegistry
    {
        private static readonly HashSet<string> RegisteredTypes = new(StringComparer.Ordinal)
        {
            InterventionConstants.AttachmentEntityType,

            // EntityType for Draft sessions
            AttachmentDraftSession.AttachmentEntityType,
        };

        public static bool IsRegistered(string entityType) => RegisteredTypes.Contains(entityType);
    }
}
