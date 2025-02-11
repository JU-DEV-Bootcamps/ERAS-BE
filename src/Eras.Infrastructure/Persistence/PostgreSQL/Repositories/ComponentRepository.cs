using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ComponentRepository : BaseRepository<Component, ComponentEntity>, IComponentRepository
    {
        public ComponentRepository(AppDbContext context)
            : base(context, ComponentMapper.ToDomain, ComponentMapper.ToPersistence)
        {
        }
    }
}