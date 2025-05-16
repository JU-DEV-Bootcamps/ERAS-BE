using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class AnswerMapper
    {
        public static Answer ToDomain(this AnswerEntity Entity) => new()
        {
            Id = Entity.Id,
            RiskLevel = Entity.RiskLevel,
            AnswerText = Entity.AnswerText,
            Audit = Entity.Audit,
            PollInstanceId = Entity.PollInstanceId,
            PollVariableId = Entity.PollVariableId,
            Version = Entity.Version
        };
        public static AnswerEntity ToPersistence(this Answer Model) => new()
        {
            Id = Model.Id,
            AnswerText = Model.AnswerText,
            RiskLevel = Model.RiskLevel,
            PollInstanceId = Model.PollInstanceId,
            PollVariableId = Model.PollVariableId,
            Audit = Model.Audit,
            Version = Model.Version
        };
    }
}
