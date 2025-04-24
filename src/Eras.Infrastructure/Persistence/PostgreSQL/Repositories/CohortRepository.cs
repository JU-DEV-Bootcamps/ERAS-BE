using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CohortRepository : BaseRepository<Cohort, CohortEntity>, ICohortRepository
    {
        public CohortRepository(AppDbContext Context)
            : base(Context, CohortMapper.ToDomain, CohortMapper.ToPersistence)
        {
        }

        public async Task<Cohort?> GetByNameAsync(string Name)
        {
            var cohort = await _context.Cohorts
                .FirstOrDefaultAsync(Cohort => Cohort.Name == Name);
            
            return cohort?.ToDomain();
        }

        public async Task<Cohort?> GetByCourseCodeAsync(string Name)
        {
            var cohort = await _context.Cohorts
                .FirstOrDefaultAsync(Cohort => Cohort.CourseCode == Name);
            
            return cohort?.ToDomain();
        }

        public async Task<List<Cohort>> GetCohortsAsync()
        {
            var cohorts = await _context.Cohorts
                .ToListAsync();
            return cohorts.Select(P => P.ToDomain()).ToList();
        }

    }
}
