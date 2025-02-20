using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollVariableRepository : BaseRepository<Variable, PollVariableJoin>, IPollVariableRepository
    {
        public PollVariableRepository(AppDbContext context )
            : base(context, PollVariableMapper.ToDomain, PollVariableMapper.ToPersistenceVariable)
        {

        }
        public async Task<Variable?> GetByPollIdAndVariableIdAsync(int pollId, int variableId)
        {
            var pollVariable = await _context.PollVariables
                .FirstOrDefaultAsync(pollVar => pollVar.PollId == pollId && pollVar.VariableId == variableId);

            return pollVariable?.ToDomain();
        }
    }
}
