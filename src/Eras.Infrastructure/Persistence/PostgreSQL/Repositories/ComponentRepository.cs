using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ComponentRepository : BaseRepository<Component, ComponentEntity>, IComponentRepository
    {
        public ComponentRepository(AppDbContext context)
            : base(context, ComponentMapper.ToDomain, ComponentMapper.ToPersistence)
        {
        }

        public async Task<Component?> GetByNameAsync(string name)
        {
            var component = await _context.Components
                .FirstOrDefaultAsync(component => component.Name == name);

            return component?.ToDomain();
        }
    }
}