using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollRepository : BaseRepository<Poll, PollEntity>, IPollRepository
    {
        public PollRepository(AppDbContext Context)
            : base(Context, PollMapper.ToDomain, PollMapper.ToPersistence) { }

        public async Task<Poll?> GetByNameAsync(string Name)
        {
            var poll = await _context.Polls.FirstOrDefaultAsync(Poll => Poll.Name == Name);

            return poll?.ToDomain();
        }

        public async Task<Poll?> GetByUuidAsync(string Uuid)
        {
            var poll = await _context.Polls
                .FirstOrDefaultAsync(Poll => Poll.Uuid == Uuid);

            return poll?.ToDomain();
        }

        public new async Task<Poll> UpdateAsync(Poll Entity)
        {

            var existingEntity = await _context.Set<PollEntity>().FindAsync(Entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = PollMapper.ToPersistence(Entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return Entity;
        }
    }
}
