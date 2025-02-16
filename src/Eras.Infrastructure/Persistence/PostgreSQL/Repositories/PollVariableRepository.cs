using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollVariableRepository : BaseRepository<Variable, PollVariableJoin>, IPollVariableRepository
    {
        public PollVariableRepository(AppDbContext context ) 
            : base(context, PollVariableMapper.ToDomain, PollVariableMapper.ToPersistenceVariable)
        {

        }
    }
    }
