using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JUServiceEntity : BaseEntity, IAuditableEntity
{
    public required string Name { get; set; }
    public AuditInfo Audit { get; set; } = default!;
}
