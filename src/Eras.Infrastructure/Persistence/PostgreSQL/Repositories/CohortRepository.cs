using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

[ExcludeFromCodeCoverage]
public class CohortRepository(AppDbContext Context) : BaseRepository<Cohort, CohortEntity>(Context, CohortMapper.ToDomain, CohortMapper.ToPersistence), ICohortRepository
{
    public async Task<Cohort?> GetByNameAsync(string Name)
    {
        CohortEntity? cohort = await _context.Cohorts
            .FirstOrDefaultAsync(Cohort => Cohort.Name == Name);
        return cohort?.ToDomain();
    }

    public async Task<Cohort?> GetByCourseCodeAsync(string Name)
    {
        CohortEntity? cohort = await _context.Cohorts
            .FirstOrDefaultAsync(Cohort => Cohort.CourseCode == Name);
        return cohort?.ToDomain();
    }

    public async Task<List<Cohort>> GetCohortsAsync()
    {
        List<CohortEntity> cohorts = await _context.Cohorts
            .ToListAsync();
        return [.. cohorts.Select(P => P.ToDomain())];
    }

    public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponentAsync(string PollUuid, string ComponentName, int CohortId)
    {
        List<GetCohortTopRiskStudentsByComponentResponse> result = await _context.ErasCalculationsByPoll
                        .Where(View => View.PollUuid == PollUuid && View.ComponentName == ComponentName && View.CohortId == CohortId)
                        .GroupBy(V => new { V.PollInstanceId, V.StudentName })
                        .Select(Group => new GetCohortTopRiskStudentsByComponentResponse
                        {
                            StudentId = Group.Key.PollInstanceId,
                            StudentName = Group.Key.StudentName,
                            RiskSum = Group.Sum(V => V.AnswerRisk)
                        })
                        .OrderByDescending(G => G.RiskSum)
                        .ToListAsync();
        return result;

    }

    public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsAsync(string PollUuid, int CohortId)
    {
        List<GetCohortTopRiskStudentsByComponentResponse> result = await _context.ErasCalculationsByPoll
                        .Where(V => V.PollUuid == PollUuid && V.CohortId == CohortId)
                        .GroupBy(V => new { V.PollInstanceId, V.StudentName })
                        .Select(G => new GetCohortTopRiskStudentsByComponentResponse
                        {
                            StudentId = G.Key.PollInstanceId,
                            StudentName = G.Key.StudentName,
                            RiskSum = G.Sum(V => V.AnswerRisk)
                        })
                        .OrderByDescending(G => G.RiskSum)
                        .ToListAsync();
        return result;
    }

    public async Task<List<Cohort>> GetCohortsByPollUuidAsync(string PollUuid)
    {
        List<CohortEntity> cohorts = await _context.ErasCalculationsByPoll
            .Where(View => View.PollUuid == PollUuid)
            .Select(View => new CohortEntity
            {
                Id = View.CohortId,
                Name = View.CohortName,
            })
            .Distinct().ToListAsync();
        return [.. cohorts.Select(P => P.ToDomain())];
    }
}
