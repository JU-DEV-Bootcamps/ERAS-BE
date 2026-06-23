using Eras.Domain.Common;

namespace Eras.Domain.Entities;

public class FeatureFlag : BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public AuditInfo Audit { get; set; } = default!;
}
