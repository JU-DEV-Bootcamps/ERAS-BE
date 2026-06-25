using Eras.Domain.Common;

namespace Eras.Domain.Entities.FeatureFlagManagement;

public class FeatureFlag : BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public AuditInfo Audit { get; set; } = new AuditInfo()
    {
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "System",
        ModifiedAt = DateTime.UtcNow
    };
}
