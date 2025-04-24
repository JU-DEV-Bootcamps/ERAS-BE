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
    }
}
