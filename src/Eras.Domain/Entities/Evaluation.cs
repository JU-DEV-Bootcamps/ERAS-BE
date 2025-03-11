using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Evaluation : BaseEntity, IAuditableEntity
    {

        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PollId { get; set; }
        public int EvaluationPollId { get; set; }
        public ICollection<Poll> Polls { get; set; } = [];

        public AuditInfo Audit { get; set; } = default!;
    }
}
