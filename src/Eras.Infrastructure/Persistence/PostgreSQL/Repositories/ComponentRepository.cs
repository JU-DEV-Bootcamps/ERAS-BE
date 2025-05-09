using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ComponentRepository : BaseRepository<Component, ComponentEntity>, IComponentRepository
    {
        public ComponentRepository(AppDbContext Context)
            : base(Context, ComponentMapper.ToDomain, ComponentMapper.ToPersistence)
        {
        }

        public new async Task<IEnumerable<Component>> GetAllAsync()
        {
            var components = await _context.Components
                .ToListAsync();
            return components.Select(Component => Component.ToDomain());
        }

        public async Task<Component?> GetByNameAsync(string Name)
        {
            var component = await _context.Components
                .FirstOrDefaultAsync(Component => Component.Name == Name);

            return component?.ToDomain();
        }

        public async Task<Component?> GetByNameAndPollIdAsync(string Name, int PollId)
        {
            var component = await _context.Components
                .Where(C => C.Name == Name)
                .Join(_context.Variables,
                    Component => Component.Id,
                    Variable => Variable.ComponentId,
                    (Component,Variable)=> new { Component,Variable})
                .Join(_context.PollVariables,
                    Cv => Cv.Variable.Id,
                    Pv => Pv.VariableId,
                    (Cv, PollVariable) => new { Cv.Component, Cv.Variable, PollVariable })
                .Where(Cv => Cv.PollVariable.PollId == PollId)
                .Select( Cv => Cv.Component)
                .FirstOrDefaultAsync();
            return component?.ToDomain();
        } 
    }
}
