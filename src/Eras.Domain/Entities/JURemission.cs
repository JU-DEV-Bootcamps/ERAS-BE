using Eras.Domain.Common;

using static Eras.Domain.Entities.JURemissionsConstants;

namespace Eras.Domain.Entities
{
    public class JURemission : BaseEntity, IAuditableEntity
    {
        public string SubmitterUuid { get; set; } = string.Empty;
        public JUService JUService { get; set; } = default!;
        public JUProfessional AssignedProfessional = default!;
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public RemissionsStatus Status { get; set; } = RemissionsStatus.Created;
        public ICollection<Student> Students { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}
