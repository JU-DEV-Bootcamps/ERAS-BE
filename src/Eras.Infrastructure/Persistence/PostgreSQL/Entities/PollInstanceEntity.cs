using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class PollInstanceEntity : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public StudentEntity Student { get; set; } = default!;
        public ICollection<AnswerEntity> Answers { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
        public DateTime FinishedAt { get; set; }
    }
}