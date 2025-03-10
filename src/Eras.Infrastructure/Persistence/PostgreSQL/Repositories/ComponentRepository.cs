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
        public ComponentRepository(AppDbContext context)
            : base(context, ComponentMapper.ToDomain, ComponentMapper.ToPersistence)
        {
        }

        public async Task<IEnumerable<Component>> GetAllAsync()
        {
            var components = await _context.Components
                .ToListAsync();
            return components.Select(component => component.ToDomain());
        }

        public async Task<Component?> GetByNameAsync(string name)
        {
            var component = await _context.Components
                .FirstOrDefaultAsync(component => component.Name == name);

            return component?.ToDomain();
        }
    }
}