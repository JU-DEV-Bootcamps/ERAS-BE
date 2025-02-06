using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollRepository : BaseRepository<Poll>, IPollRepository
    {
        public PollRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Poll?> GetByNameAsync(string name)
        {
            var poll = await _context.Polls
                .FirstOrDefaultAsync(poll => poll.Name == name);
            
            return poll;
        }
    }
}