using Eras.Domain.Common;

using static Eras.Domain.Entities.JURemissionsConstants;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JURemissionEntity : BaseEntity, IAuditableEntity
{
    public required string SubmitterUuid { get; set; }
    public int JUServiceId { get; set; }
    public required int AssignedProfessionalId { get; set; }
    public JUProfessionalEntity AssignedProfessional { get; set; } = default!;
    public int StudentId { get; set; }
    public required ICollection<StudentEntity> Students { get; set; } = [];
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public RemissionsStatus Status { get; set; } = RemissionsStatus.Created;
    public AuditInfo Audit { get; set; } = default!;
}
