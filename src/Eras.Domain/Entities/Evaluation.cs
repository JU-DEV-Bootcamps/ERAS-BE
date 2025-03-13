using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Evaluation : BaseEntity, IAuditableEntity
    {

        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty ;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PollName { get; set; }= string.Empty ;
        public int PollId { get; set; }
        public int EvaluationPollId { get; set; }
        public ICollection<Poll> Polls { get; set; } = [];
        public ICollection<PollInstance>? PollInstances { get; set; }
        public AuditInfo Audit { get; set; } = default!;
    }
}
