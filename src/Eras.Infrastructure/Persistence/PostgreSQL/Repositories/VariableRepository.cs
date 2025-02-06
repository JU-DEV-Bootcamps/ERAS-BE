using Eras.Domain.Entities;
using Eras.Domain.Repositories;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class VariableRepository : BaseRepository<Variable>, IVariableRepository
    {
        public VariableRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}