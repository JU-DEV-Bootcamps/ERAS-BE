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

        public async Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? cohortId, int? days)
        {
            IQueryable<PollInstanceEntity> query = _context.PollInstances.Include(pi => pi.Student);

            if (cohortId.HasValue && cohortId != 0)
            {
                query = query
                    .Join(_context.StudentCohorts,
                        pollInstance => pollInstance.StudentId,
                        studentCohort => studentCohort.StudentId,
                        (pollInstance, studentCohort) => new { pollInstance, studentCohort })
                    .Where(joined => joined.studentCohort.CohortId == cohortId.Value)
                    .Select(joined => joined.pollInstance);
            }

            if (days.HasValue && days != 0)
            {
                var dateLimit = DateTime.UtcNow.AddDays(-days.Value);
                query = query.Where(pi => pi.FinishedAt >= dateLimit);
            }

            var pollInstances = await query.OrderByDescending(pi => pi.FinishedAt).Distinct().ToListAsync();
            return pollInstances.Select(pi => PollInstanceMapper.ToDomain(pi)).ToList();
        }

        public async Task<IEnumerable<PollInstance>> GetByCohortId(int cohortId)
        {
            var polls = await _context.Cohorts
                .Where(c => c.Id == cohortId)
                .Join(_context.StudentCohorts,
                    cohort => cohort.Id,
                    studentCohort => studentCohort.CohortId,
                    (cohort, studentCohort) => studentCohort)
                .Join(_context.Students,
                    sc => sc.StudentId,
                    student => student.Id,
                    (sc, student) => student)
                .Join(_context.PollInstances,
                    student => student.Id,
                    pollInstance => pollInstance.StudentId,
                    (student, pollInstance) => pollInstance)
                .Distinct()
                .ToListAsync();

            return polls.Select(p => p.ToDomain()).ToList();
        }

    }
}