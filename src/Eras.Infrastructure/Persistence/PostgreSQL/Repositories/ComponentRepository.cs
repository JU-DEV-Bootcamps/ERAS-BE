using Eras.Domain.Entities;
using Eras.Domain.Repositories;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ComponentRepository : BaseRepository<Component>, IComponentRepository
    {
        public ComponentRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}