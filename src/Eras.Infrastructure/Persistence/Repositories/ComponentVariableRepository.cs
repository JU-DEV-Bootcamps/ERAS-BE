using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class ComponentVariableRepository : IComponentVariableRepository<ComponentVariable>
    {
        public Task<ComponentVariable> Add(ComponentVariable componentVariable)
        {
            throw new NotImplementedException();
        }

        public Task<List<ComponentVariable>> GetAll(int pollId)
        {
            throw new NotImplementedException();
        }

        public Task<ComponentVariable> GetComponentVariableByName(string name)
        {
            throw new NotImplementedException();
        }

    }
}
