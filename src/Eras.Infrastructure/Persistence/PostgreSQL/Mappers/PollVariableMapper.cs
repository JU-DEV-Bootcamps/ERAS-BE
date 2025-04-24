using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                PollVariableId = PollVariableJoin.Id 

            };
        }
        public static PollVariableJoin ToPersistenceVariable(this Variable Variable)
        {
            return new PollVariableJoin()
            {
                VariableId = Variable.Id,
                PollId = Variable.IdPoll,
                
            };
        }
    }
}
