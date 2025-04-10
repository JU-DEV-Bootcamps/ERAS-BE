using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Joins
{
    public class EvaluationPollJoin : BaseEntity
    {
        public int PollId { get; set; }

        public PollEntity Poll { get; set; } = default!;
        public int EvaluationId { get; set; }

        public EvaluationEntity Evaluation { get; set; } = default!;

    }
}
