using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class PollVersionEntity: BaseEntity, IAuditableEntity
    {
        public string Name = string.Empty;
        public DateTime Date;
        public int PollId;
        public PollEntity Poll { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
    }
}
