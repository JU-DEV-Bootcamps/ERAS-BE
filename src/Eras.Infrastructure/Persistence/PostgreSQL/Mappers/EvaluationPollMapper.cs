using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class EvaluationPollMapper
    {
        public static Evaluation ToDomain(this EvaluationPollJoin Entity)
        {
            return new Evaluation
            {
                Id = Entity.EvaluationId,
                PollId = Entity.PollId,
                EvaluationPollId = Entity.Id,
            };
        }

        public static EvaluationPollJoin ToPersistence(this Evaluation Model)
        {
            return new EvaluationPollJoin
            {
                Id = Model.EvaluationPollId,
                PollId = Model.PollId,
                EvaluationId = Model.Id,
            };
        }
    }
}
