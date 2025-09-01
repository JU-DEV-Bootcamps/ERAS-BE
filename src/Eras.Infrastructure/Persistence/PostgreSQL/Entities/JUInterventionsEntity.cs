using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JUInterventionsEntity : BaseEntity, IAuditableEntity
{
    public int StudentId { get; set; }
    public required StudentEntity Student { get; set; }
    public string Diagnostic { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public IEnumerable<JURemissionEntity> RemissionsList { get; set; } = [];
    public required AuditInfo Audit { get; set; }
}
