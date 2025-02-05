using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class VariableRepository : BaseRepository<Variable>, IVariableRepository<Variable>
    {
        public VariableRepository(AppDbContext context)
            : base(context)
        {
        }

        public Task<Variable> Add(Variable componentVariable)
        {
            throw new NotImplementedException();
        }

        public Task<List<Variable>> GetAll(int pollId)
        {
            throw new NotImplementedException();
        }

        public Task<Variable> GetComponentVariableByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}