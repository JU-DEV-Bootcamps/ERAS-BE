using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ComponentVariable> GetComponentVariableByName(string name)
        {
            var variableByName = await _context.ComponentVariables.Where(v => v.Name.Equals(name)).ToListAsync();           
            if (variableByName.Count > 0) return variableByName[0].ToComponentVariable();
            return null;
        }

        public async Task<ComponentVariable> Add(ComponentVariable componentVariable)
        {
            // TODO check: partent Id, how we are going to handle this?
            // TODO check: Only check by name? Or maybe we need to had a consideration with poll id?
            var existingVariable = await GetComponentVariableByName(componentVariable.Name);
            if (existingVariable != null) return existingVariable;
                
            var componentVariableEntity = componentVariable.ToComponentVariableEntity();
            _context.ComponentVariables.Add(componentVariableEntity);
            await _context.SaveChangesAsync();
            return componentVariableEntity.ToComponentVariable();
        }

        public async Task<List<ComponentVariable>> GetAll(int pollId)
        {
            List<ComponentVariableEntity> list =  await _context.ComponentVariables.Where(v => v.Poll.Id == pollId).ToListAsync();
            List<ComponentVariable> result = new List<ComponentVariable>();
            foreach (var item in list)
            {
                result.Add(item.ToComponentVariable());
            }
            return result;

        }
    }
}
