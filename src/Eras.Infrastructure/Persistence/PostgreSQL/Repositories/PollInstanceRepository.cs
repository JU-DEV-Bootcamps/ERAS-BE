using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollInstanceRepository : BaseRepository<PollInstance>, IPollInstanceRepository
    {
        public PollInstanceRepository(AppDbContext context) : base(context)
        {
        }

    public async Task<PollInstance?> GetByUuidAsync(string uuid)
    {
        var pollInstance = await _context.PollInstances 
            .FirstOrDefaultAsync(pollInstance => pollInstance.Uuid == uuid);
        
        return pollInstance;
    }
  }
}