using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class EvaluationEntity: BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty ;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<EvaluationPollJoin> EvaluationPolls { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}
