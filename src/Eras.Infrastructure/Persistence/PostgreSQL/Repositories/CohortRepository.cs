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
   
    public async Task<IEnumerable<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponentAsync(
    string PollUuid, string ComponentName, int CohortId, bool LastVersion, int Page, int PageSize)
    {
        var query = BuildCohort(PollUuid, CohortId, LastVersion, ComponentName);
        return await ProjectAndPaginate(query, Page, PageSize).ToListAsync();
    }

    public async Task<IEnumerable<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsAsync(
        string PollUuid, int CohortId, bool LastVersion, int Page, int PageSize)
    {
        var query = BuildCohort(PollUuid, CohortId, LastVersion);
        return await ProjectAndPaginate(query, Page, PageSize).ToListAsync();
    }

    public async Task<int> CountStudentsAsync(string PollUuid, int CohortId, bool LastVersion, string? ComponentName = null)
    {
        var query = BuildCohort(PollUuid, CohortId, LastVersion, ComponentName);
        return await query
            .GroupBy(V => new { V.PollInstanceId, V.StudentName, V.StudentId })
            .CountAsync();
    }

    private IQueryable<GetCohortTopRiskStudentsByComponentResponse> ProjectAndPaginate(
        IQueryable<ErasCalculationsByPollEntity> Query, int Page, int PageSize)
    {
        return Query
            .GroupBy(V => new { V.PollInstanceId, V.StudentName, V.StudentId })
            .Select(G => new GetCohortTopRiskStudentsByComponentResponse
            {
                StudentId = G.Key.StudentId,
                StudentName = G.Key.StudentName,
                AnswerAverage = Math.Round((decimal)G.Average(V => V.AnswerRisk), 2),
                RiskSum = G.Sum(V => V.AnswerRisk)
            })
            .OrderByDescending(G => G.RiskSum)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize);
    }

    private IQueryable<ErasCalculationsByPollEntity> BuildCohort(
        string PollUuid, int CohortId, bool LastVersion, string? ComponentName = null)
    {
        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        var query = _context.ErasCalculationsByPoll
            .Where(V => V.PollUuid == PollUuid && V.CohortId == CohortId);

        query = LastVersion
            ? query.Where(v => v.PollVersion == pollVersion)
            : query.Where(v => v.PollVersion != pollVersion);

        if (!string.IsNullOrEmpty(ComponentName))
            query = query.Where(v => v.ComponentName == ComponentName);

        return query;
    }

    public async Task<List<Cohort>> GetCohortsByPollUuidAsync(string PollUuid, bool LastVersion)
    {
        int lastPollVersion = _context.Polls.Where(A => A.Uuid == PollUuid).Select(A => A.LastVersion).FirstOrDefault();
        IQueryable<CohortEntity> query;
        List<CohortEntity> cohorts;

        if (LastVersion)
        {
            query = _context.ErasCalculationsByPoll
                .Where(View => View.PollUuid == PollUuid && View.PollVersion == lastPollVersion)
                .Select(View => new CohortEntity
                {
                    Id = View.CohortId,
                    Name = View.CohortName,
                });
        }
        else
        {
            query = _context.ErasCalculationsByPoll
                .Where(View => View.PollUuid == PollUuid && View.PollVersion != lastPollVersion)
                .Select(View => new CohortEntity
                {
                    Id = View.CohortId,
                    Name = View.CohortName,
                });
        }
        cohorts = await query.Distinct().ToListAsync();

        return [.. cohorts.Select(P => P.ToDomain())];
    }

    public async Task<List<Cohort>> GetCohortsByPollIdAsync(int PollId)
    {
        List<CohortEntity> cohorts = await _context.ErasCalculationsByPoll
            .Where(View => View.PollId == PollId)
            .Select(View => new CohortEntity
            {
                Id = View.CohortId,
                Name = View.CohortName,
            })
            .Distinct().ToListAsync();
        return [.. cohorts.Select(P => P.ToDomain())];
    }
}
