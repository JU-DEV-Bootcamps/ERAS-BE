using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JUProfessionalEntity: BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public AuditInfo Audit { get; set; } = default!;
}