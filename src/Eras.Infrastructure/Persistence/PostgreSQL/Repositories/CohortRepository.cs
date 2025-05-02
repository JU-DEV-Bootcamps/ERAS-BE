using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
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

        public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponent(string PollUuid, string ComponentName, int CohortId)
        {
            var result = _context.ErasCalculationsByPoll
                            .Where(v => v.PollUuid == PollUuid && v.ComponentName == ComponentName && v.CohortId == CohortId)
                            .GroupBy(v => new { v.PollInstanceId, v.Name }) 
                            .Select(g => new GetCohortTopRiskStudentsByComponentResponse
                            {
                                StudentId = g.Key.PollInstanceId,
                                StudentName = g.Key.Name,        
                                RiskSum = g.Sum(v => v.RiskSum) 
                            })
                            .OrderByDescending(g => g.RiskSum)
                            .ToList();
            return result;

        }

        public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudents(string PollUuid, int CohortId)
        {
            var result = _context.ErasCalculationsByPoll
                            .Where(v => v.PollUuid == PollUuid && v.CohortId == CohortId)
                            .GroupBy(v => new { v.PollInstanceId, v.Name })
                            .Select(g => new GetCohortTopRiskStudentsByComponentResponse
                            {
                                StudentId = g.Key.PollInstanceId,
                                StudentName = g.Key.Name,
                                RiskSum = g.Sum(v => v.RiskSum)
                            })
                            .OrderByDescending(g => g.RiskSum)
                            .ToList();
            return result;
        }
    }
}