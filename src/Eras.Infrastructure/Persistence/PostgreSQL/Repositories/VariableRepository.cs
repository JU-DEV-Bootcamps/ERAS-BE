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

        public VariableRepository(AppDbContext Context)
            : base(Context, VariableMapper.ToDomain, VariableMapper.ToPersistence)
        {
        }

        public async Task<Variable?> GetByNameAsync(string Name)
        {
            var variable = await _context.Variables
                .FirstOrDefaultAsync(Variable => Variable.Name == Name);

            return variable?.ToDomain();
        }
        public new async Task<Variable> AddAsync(Variable Variable)
        {

            VariableEntity variableEntity = Variable.ToPersistence();
            var response = await _context.Set<VariableEntity>().AddAsync(variableEntity);
            await _context.SaveChangesAsync();

            Variable variableCreated = response.Entity.ToDomain();




            return variableCreated;
        }
        public Task<Variable> Add(Variable ComponentVariable)
        {
            throw new NotImplementedException();
        }

        public Task<List<Variable>> GetAllAsync(int PollId)
        {
            throw new NotImplementedException();
        }

        public Task<Variable> GetComponentVariableByName(string Name)
        {
            throw new NotImplementedException();
        }
    }
}
