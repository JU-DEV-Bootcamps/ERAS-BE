using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class EvaluationPollMapper
    {
        public static Evaluation ToDomain(this EvaluationPollJoin entity)
        {
            return new Evaluation
            {
                Id = entity.EvaluationId,
                PollId = entity.PollId,
                EvaluationPollId = entity.Id,
            };
        }

        public static EvaluationPollJoin ToPersistence(this Evaluation model)
        {
            return new EvaluationPollJoin
            {
                Id = model.EvaluationPollId,
                PollId = model.PollId,
                EvaluationId = model.Id,
            };
        }
    }
}
