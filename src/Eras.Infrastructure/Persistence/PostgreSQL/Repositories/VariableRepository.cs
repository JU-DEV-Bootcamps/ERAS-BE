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
            List<string> Components,
            bool LastVersion
        )
        {
            int pollVersion = _context.Polls
            .AsNoTracking()
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();
            
            var query = _context.Variables
                .Join(_context.PollVariables, V => V.Id, Pv => Pv.VariableId, (V, Pv) => new { V, Pv })
                .Join(_context.Polls, Vp => Vp.Pv.PollId, P => P.Id, (Vp, P) => new { Vp.V, Vp.Pv, P })
                .Join(_context.Components, Vpp => Vpp.V.ComponentId, C => C.Id, (Vpp, C) => new { Vpp.V, Vpp.Pv, Vpp.P, C })
                .Where(X => X.P.Uuid == PollUuid
                    && (Components.Count == 0 || Components.Contains(X.C.Name))
                    && (LastVersion ? X.Pv.Version.VersionNumber == pollVersion : X.Pv.Version.VersionNumber != pollVersion))
                .Select(X => new Variable
                {
                    Id = X.V.Id,
                    Name = X.V.Name,
                    ComponentName = X.C.Name,
                    Position = X.V.Position,
                    Audit = X.V.Audit,
                    IdComponent = X.C.Id,
                    PollVariableId = X.Pv.Id,
                    IdPoll = X.P.Id
                })
                .OrderBy(X => X.Position);

            return await query.AsNoTracking().ToListAsync();
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

        public async Task<Variable?> GetByNameAndPositionAsync(string Name, int Position)
        {
            var variable = await _context.Variables.FirstOrDefaultAsync(Variable =>
                Variable.Name == Name && Variable.Position == Position
            );

            return variable?.ToDomain();
        }
    }
}
