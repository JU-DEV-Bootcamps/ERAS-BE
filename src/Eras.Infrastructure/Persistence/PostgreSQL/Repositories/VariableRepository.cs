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
            : base(Context, VariableMapper.ToDomain, VariableMapper.ToPersistence) { }

        public async Task<Variable?> GetByNameAsync(string Name)
        {
            var variable = await _context.Variables.FirstOrDefaultAsync(Variable =>
                Variable.Name == Name
            );

            return variable?.ToDomain();
        }

        public async Task<Variable?> GetByNameInPollAndComponentAsync(string Name)
        {
            var variable = await _context.Variables.FirstOrDefaultAsync(Variable =>
                Variable.Name == Name
            );

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

        public async Task<List<Variable>> GetAllByPollUuidAsync(
            string PollUuid,
            List<string> Components
        )
        {
            var variables =
                from v in _context.Variables
                join pv in _context.PollVariables on v.Id equals pv.VariableId
                join p in _context.Polls on pv.PollId equals p.Id
                join c in _context.Components on v.ComponentId equals c.Id
                where p.Uuid == PollUuid && Components.Contains(c.Name)
                select new Variable { Id = v.Id, Name = v.Name };

            return await variables.ToListAsync();
        }

        public async Task<Variable?> GetByNameAndPollIdAsync(string Name, int PollId)
        {
            var variable = await _context.Variables
                .Where(V => V.Name == Name)
                .Join(_context.PollVariables,
                    V => V.Id,
                    Pv => Pv.VariableId,
                    (Variable, PollVariable) => new {Variable, PollVariable })
                .Where(Cv => Cv.PollVariable.PollId == PollId)
                .Select(Cv => Cv.Variable)
                .FirstOrDefaultAsync();
            return variable?.ToDomain();
        }
    }
}
