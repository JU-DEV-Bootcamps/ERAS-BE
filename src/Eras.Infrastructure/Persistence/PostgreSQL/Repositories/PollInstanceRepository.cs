using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollInstanceRepository : BaseRepository<PollInstance, PollInstanceEntity>, IPollInstanceRepository
    {
        public PollInstanceRepository(AppDbContext context) 
            : base(context, PollInstanceMapper.ToDomain, PollInstanceMapper.ToPersistence)
        {
        }

        public async Task<PollInstance?> GetByUuidAsync(string uuid)
        {
            var pollInstance = await _context.PollInstances 
                .FirstOrDefaultAsync(pollInstance => pollInstance.Uuid == uuid);
        
            return pollInstance?.ToDomain();
        }

        public async Task<PollInstance?> GetByUuidAndStudentIdAsync(string uuid, int studentId)
        {
            var results = await _context.PollInstances.FirstOrDefaultAsync(poll => poll.Uuid.Equals(uuid) && poll.StudentId.Equals(studentId));
            return results?.ToDomain();
        }


        public async Task<IEnumerable<PollInstance>> GetByLastDays(int days)
        {
            var dateLimit = DateTime.UtcNow.AddDays(-days);
            var pollInstanceCounts = await _context.PollInstances
            .Include(pi => pi.Student)
            .Where(pi => pi.FinishedAt >= dateLimit)
            .ToListAsync();

            return pollInstanceCounts.Select(entity => PollInstanceMapper.ToDomain(entity));
        }

    }
}