using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class PollInstance : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;
        public ICollection<Answer> Answers { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}
