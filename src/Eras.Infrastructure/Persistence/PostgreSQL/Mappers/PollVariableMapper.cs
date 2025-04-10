using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class PollVariableMapper
    {
        public static Variable ToDomain(this PollVariableJoin pollVariableJoin)
        {
            return new Variable()
            {
                Id = pollVariableJoin.VariableId,
                IdPoll = pollVariableJoin.PollId,
                IdComponent = pollVariableJoin.Variable.ComponentId,
                PollVariableId = pollVariableJoin.Id

            };
        }
        public static PollVariableJoin ToPersistenceVariable(this Variable variable)
        {
            return new PollVariableJoin()
            {
                VariableId = variable.Id,
                PollId = variable.IdPoll,

            };
        }
    }
}
