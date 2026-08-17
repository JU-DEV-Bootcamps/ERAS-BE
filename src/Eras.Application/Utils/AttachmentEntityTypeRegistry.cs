using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Utils
{
    /// <summary>
    /// Whitelist of `entityType` values allowed to own attachments through the generic
    /// <c>AttachmentService</c> (User Story 1.4). A new entity type must be added here explicitly
    /// before it can upload/list/download/delete attachments — this is a deliberate, compiled
    /// gate, not a runtime-configurable list, since supporting a new entity type always requires
    /// real code changes elsewhere anyway (DTOs, permission checks, etc.).
    /// </summary>
    public static class AttachmentEntityTypeRegistry
    {
        private static readonly HashSet<string> RegisteredTypes = new(StringComparer.Ordinal)
        {
            InterventionConstants.AttachmentEntityType,
        };

        public static bool IsRegistered(string entityType) => RegisteredTypes.Contains(entityType);
    }
}
