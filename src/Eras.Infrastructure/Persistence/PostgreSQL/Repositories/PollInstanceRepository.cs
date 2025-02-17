using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
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
            var pollInstance = await _context.PollInstances
                .FirstOrDefaultAsync(pollInstance => pollInstance.Uuid == uuid && pollInstance.StudentId == studentId);

            return pollInstance?.ToDomain();
        }

    }
}