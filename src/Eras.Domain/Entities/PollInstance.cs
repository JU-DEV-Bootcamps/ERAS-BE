using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class PollInstance : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = string.Empty;
        public Student? Student { get; set; } = default!;
        public ICollection<Answer> Answers { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
        public int LastVersion { get; set; }
        public DateTime LastVersionDate { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}
