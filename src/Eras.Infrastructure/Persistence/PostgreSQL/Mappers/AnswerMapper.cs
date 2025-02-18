using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class AnswerMapper
    {
        public static Answer ToDomain(this AnswerEntity entity)
        {
            return new Answer
            {
                Id = entity.Id,
                RiskLevel = entity.RiskLevel,
                AnswerText = entity.AnswerText,
                Audit = entity.Audit,
                PollInstanceId = entity.PollInstanceId,
                PollVariableId = entity.PollVariableId
            };
        }

        public static AnswerEntity ToPersistence(this Answer model)
        {
            return new AnswerEntity
            {
                Id = model.Id,
                AnswerText = model.AnswerText,
                RiskLevel = model.RiskLevel,
                PollInstanceId = model.PollInstanceId,
                PollVariableId = model.PollVariableId,
                Audit = model.Audit

            };
        }
    }
}