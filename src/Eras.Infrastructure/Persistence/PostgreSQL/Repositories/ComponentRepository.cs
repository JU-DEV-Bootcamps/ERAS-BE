using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

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