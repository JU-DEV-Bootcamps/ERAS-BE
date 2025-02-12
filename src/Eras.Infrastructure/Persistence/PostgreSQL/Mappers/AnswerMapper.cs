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
                Audit = entity.Audit
            };
        }

        public static AnswerEntity ToPersistence(this Answer model)
        {
            return new AnswerEntity
            {
                Id = model.Id,
                RiskLevel = model.RiskLevel,
                Audit = model.Audit
            };
        }
    }
}