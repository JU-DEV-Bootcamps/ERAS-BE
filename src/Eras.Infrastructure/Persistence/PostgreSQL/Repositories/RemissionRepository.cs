using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RemissionRepository : BaseRepository<JURemission, JURemissionEntity>, IRemissionRepository
    {
        public RemissionRepository(AppDbContext Context)
            : base(Context, JURemissionMapper.ToDomain, JURemissionMapper.ToPersistence) { }

        public async Task<JURemission?> GetBySubmitterUuidAsync(string SubmitterUuid)
        {
            var remission = await _context.Remissions
                .FirstOrDefaultAsync(Remission => Remission.SubmitterUuid == SubmitterUuid);

            return remission?.ToDomain();
        }

        public new async Task<JURemission?> GetByIdAsync(int Id)
        {
            var remission = await _context.Remissions
                .FirstOrDefaultAsync(Remission => Remission.Id == Id);

            return remission?.ToDomain();
        }
        public new async Task<JURemission> UpdateAsync(JURemission Entity)
        {

            var existingEntity = await _context.Set<JURemissionEntity>().FindAsync(Entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = JURemissionMapper.ToPersistence(Entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return Entity;
        }
    }
}
