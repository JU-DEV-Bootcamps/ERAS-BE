using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class AnswerMapper
    {
        public static Answer ToDomain(this AnswerEntity Entity)
        {
            return new Answer
            {
                Id = Entity.Id,
                RiskLevel = Entity.RiskLevel,
                AnswerText = Entity.AnswerText,
                Audit = Entity.Audit,
                PollInstanceId = Entity.PollInstanceId,
                PollVariableId = Entity.PollVariableId
            };
        }

        public static AnswerEntity ToPersistence(this Answer Model)
        {
            return new AnswerEntity
            {
                Id = Model.Id,
                AnswerText = Model.AnswerText,
                RiskLevel = Model.RiskLevel,
                PollInstanceId = Model.PollInstanceId,
                PollVariableId = Model.PollVariableId,
                Audit = Model.Audit

            };
        }
    }
}
