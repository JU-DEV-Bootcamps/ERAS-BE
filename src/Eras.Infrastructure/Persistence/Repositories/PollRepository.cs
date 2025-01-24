using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class PollRepository : IPollRepository<Poll>
    {
        private readonly AppDbContext _context;

        public PollRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Poll poll)
        {
            var polltEntity = poll.ToPollEntity();
            _context.Polls.Add(polltEntity);
            await _context.SaveChangesAsync();
        }
    }
}
