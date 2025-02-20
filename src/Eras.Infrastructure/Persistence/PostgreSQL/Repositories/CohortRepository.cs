using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class CohortRepository : BaseRepository<Cohort, CohortEntity>, ICohortRepository
    {
        public CohortRepository(AppDbContext context) 
            : base(context, CohortMapper.ToDomain, CohortMapper.ToPersistence)
        {
        }

        public async Task<Cohort?> GetByNameAsync(string name)
        {
            var cohort = await _context.Cohorts
                .FirstOrDefaultAsync(cohort => cohort.Name == name);
            
            return cohort?.ToDomain();
        }

        public async Task<Cohort?> GetByCourseCodeAsync(string courseCode)
        {
            var cohort = await _context.Cohorts
                .FirstOrDefaultAsync(cohort => cohort.CourseCode == courseCode);
            
            return cohort?.ToDomain();
        }

        public async Task<List<Cohort>> GetCohortsAsync()
        {
            var cohorts = await _context.Cohorts
                .ToListAsync();
            return cohorts.Select(p => p.ToDomain()).ToList();
        }

    }
}