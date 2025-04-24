using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollInstanceRepository : BaseRepository<PollInstance, PollInstanceEntity>, IPollInstanceRepository
    {
        public PollInstanceRepository(AppDbContext Context)
            : base(Context, PollInstanceMapper.ToDomain, PollInstanceMapper.ToPersistence)
        {
        }

        public async Task<PollInstance?> GetByUuidAsync(string Uuid)
        {
            var pollInstance = await _context.PollInstances 
                .FirstOrDefaultAsync(PollInstance => PollInstance.Uuid == Uuid);
        
            return pollInstance?.ToDomain();
        }

        public async Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId)
        {
            var results = await _context.PollInstances.FirstOrDefaultAsync(Poll => Poll.Uuid.Equals(Uuid) && Poll.StudentId.Equals(StudentId));
            return results?.ToDomain();
        }


        public async Task<IEnumerable<PollInstance>> GetByLastDays(int Days)
        {
            var dateLimit = DateTime.UtcNow.AddDays(-Days);
            var pollInstanceCounts = await _context.PollInstances
            .Include(Pi => Pi.Student)
            .Where(Pi => Pi.FinishedAt >= dateLimit)
            .ToListAsync();

            return pollInstanceCounts.Select(Entity => PollInstanceMapper.ToDomain(Entity));
        }

        public async Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days)
        {
            IQueryable<PollInstanceEntity> query = _context.PollInstances.Include(Pi => Pi.Student);

            if (CohortId.HasValue && CohortId != 0)
            {
                query = query
                    .Join(_context.StudentCohorts,
                        PollInstance => PollInstance.StudentId,
                        StudentCohort => StudentCohort.StudentId,
                        (PollInstance, studentCohort) => new { pollInstance = PollInstance, studentCohort })
                    .Where(Joined => Joined.studentCohort.CohortId == CohortId.Value)
                    .Select(Joined => Joined.pollInstance);
            }

            if (Days.HasValue && Days != 0)
            {
                var dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
                query = query.Where(Pi => Pi.FinishedAt >= dateLimit);
            }

            var pollInstances = await query.Distinct().ToListAsync();
            return pollInstances.Select(Pi => PollInstanceMapper.ToDomain(Pi)).ToList();
        }
    }
}
