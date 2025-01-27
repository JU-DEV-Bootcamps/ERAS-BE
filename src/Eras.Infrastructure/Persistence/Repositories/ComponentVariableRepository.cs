using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class ComponentVariableRepository : IComponentVariableRepository<ComponentVariable>
    {
        private readonly AppDbContext _context;

        public ComponentVariableRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ComponentVariable> Add(ComponentVariable componentVariable)
        {
            var componentVariableEntity = componentVariable.ToComponentVariableEntity();
            _context.ComponentVariables.Add(componentVariableEntity);
            await _context.SaveChangesAsync();
            return componentVariableEntity.ToComponentVariable();
        }
    }
}
