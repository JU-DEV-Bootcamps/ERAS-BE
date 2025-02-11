using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollRepository : BaseRepository<Poll, PollEntity>, IPollRepository
    {
        public PollRepository(AppDbContext context) 
            : base(context, PollMapper.ToDomain, PollMapper.ToPersistence)
        {
        }

        public async Task<Poll?> GetByNameAsync(string name)
        {
            var poll = await _context.Polls
                .FirstOrDefaultAsync(poll => poll.Name == name);
            
            return poll?.ToDomain();
        }
    }
}