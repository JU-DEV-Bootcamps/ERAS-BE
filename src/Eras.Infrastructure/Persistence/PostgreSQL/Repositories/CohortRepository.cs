using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class CohortRepository : BaseRepository<Cohort>, ICohortRepository
    {
        public CohortRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Cohort?> GetByNameAsync(string name)
        {
            var cohort = await _context.Cohorts
                .FirstOrDefaultAsync(cohort => cohort.Name == name);
            
            return cohort;
        }

        public async Task<Cohort?> GetByCourseCodeAsync(string courseCode)
        {
            var cohort = await _context.Cohorts
                .FirstOrDefaultAsync(cohort => cohort.CourseCode == courseCode);
            
            return cohort;
        }
    }
}