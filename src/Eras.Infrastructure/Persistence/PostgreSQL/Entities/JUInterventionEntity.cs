using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JUInterventionEntity : BaseEntity, IAuditableEntity
{
    public int StudentId { get; set; }
    public string Diagnostic { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public IEnumerable<JURemissionEntity> Remissions { get; set; } = [];
    public required AuditInfo Audit { get; set; }
}
