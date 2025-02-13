using System.Text;
using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Student: BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; }= string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public StudentDetail StudentDetail { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
    }
}


