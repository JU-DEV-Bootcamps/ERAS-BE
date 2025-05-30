using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class PollVariableMapper
    {
        public static Variable ToDomain(this PollVariableJoin PollVariableJoin)
        {
            return new Variable()
            {
                Id = PollVariableJoin.VariableId,
                IdPoll = PollVariableJoin.PollId,
                IdComponent = PollVariableJoin.Variable.ComponentId,
                PollVariableId = PollVariableJoin.Id,
                Version = PollVariableJoin.Version,

            };
        }
        public static PollVariableJoin ToPersistenceVariable(this Variable Variable)
        {
            return new PollVariableJoin()
            {
                VariableId = Variable.Id,
                PollId = Variable.IdPoll,
                Version = Variable.Version,

            };
        }
    }
}
