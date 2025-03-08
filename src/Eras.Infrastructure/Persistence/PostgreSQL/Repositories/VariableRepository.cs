using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class VariableRepository : BaseRepository<Variable, VariableEntity>, IVariableRepository
    {

        public VariableRepository(AppDbContext context)
            : base(context, VariableMapper.ToDomain, VariableMapper.ToPersistence)
        {
        }

        public async Task<Variable?> GetByNameAsync(string name)
        {
            var variable = await _context.Variables
                .FirstOrDefaultAsync(variable => variable.Name == name);

            return variable?.ToDomain();
        }
        public async Task<Variable> AddAsync(Variable variable)
        {

            VariableEntity variableEntity = variable.ToPersistence();
            var response = await _context.Set<VariableEntity>().AddAsync(variableEntity);
            await _context.SaveChangesAsync();

            Variable variableCreated = response.Entity.ToDomain();




            return variableCreated;
        }
        public Task<Variable> Add(Variable componentVariable)
        {
            throw new NotImplementedException();
        }

        public Task<List<Variable>> GetAllAsync(int pollId)
        {
            throw new NotImplementedException();
        }

        public Task<Variable> GetComponentVariableByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}