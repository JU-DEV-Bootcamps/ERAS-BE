using Eras.Domain.Common;
public class ProfessionalEntity: BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public AuditInfo Audit { get; set; } = default!;
}