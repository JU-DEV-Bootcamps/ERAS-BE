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

    public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsByComponentAsync(string PollUuid, string ComponentName, int CohortId, bool LastVersion)
    {

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        if (LastVersion)
        {
            List<GetCohortTopRiskStudentsByComponentResponse> result = await _context.ErasCalculationsByPoll
                        .Where(View => View.PollUuid == PollUuid && View.ComponentName == ComponentName && View.CohortId == CohortId && View.PollVersion == pollVersion)
                        .GroupBy(V => new { V.PollInstanceId, V.StudentName, V.StudentId })
                        .Select(Group => new GetCohortTopRiskStudentsByComponentResponse
                        {
                            StudentId = Group.Key.StudentId,
                            StudentName = Group.Key.StudentName,
                            AnswerAverage = Math.Round((decimal)Group.Average(V => V.AnswerRisk), 2),
                            RiskSum = Group.Sum(V => V.AnswerRisk)
                        })
                        .OrderByDescending(G => G.RiskSum)
                        .ToListAsync();
            return result;
        }
        else
        {
            List<GetCohortTopRiskStudentsByComponentResponse> result = await _context.ErasCalculationsByPoll
                        .Where(View => View.PollUuid == PollUuid && View.ComponentName == ComponentName && View.CohortId == CohortId && View.PollVersion != pollVersion)
                        .GroupBy(V => new { V.PollInstanceId, V.StudentName, V.StudentId })
                        .Select(Group => new GetCohortTopRiskStudentsByComponentResponse
                        {
                            StudentId = Group.Key.StudentId,
                            StudentName = Group.Key.StudentName,
                            AnswerAverage = Math.Round((decimal)Group.Average(V => V.AnswerRisk), 2),
                            RiskSum = Group.Sum(V => V.AnswerRisk)
                        })
                        .OrderByDescending(G => G.RiskSum)
                        .ToListAsync();
            return result;
        }

        

    }

    public async Task<List<GetCohortTopRiskStudentsByComponentResponse>> GetCohortTopRiskStudentsAsync(string PollUuid, int CohortId, bool LastVersion)
    {
        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        if (LastVersion)
        {
            List<GetCohortTopRiskStudentsByComponentResponse> result = await _context.ErasCalculationsByPoll
                            .Where(V => V.PollUuid == PollUuid && V.CohortId == CohortId && V.PollVersion == pollVersion)
                            .GroupBy(V => new { V.PollInstanceId, V.StudentName, V.StudentId })
                            .Select(G => new GetCohortTopRiskStudentsByComponentResponse
                            {
                                StudentId = G.Key.StudentId,
                                StudentName = G.Key.StudentName,
                                AnswerAverage = Math.Round((decimal)G.Average(V => V.AnswerRisk), 2),
                                RiskSum = G.Sum(V => V.AnswerRisk)
                            })
                            .OrderByDescending(G => G.RiskSum)
                            .ToListAsync();
            return result;
        }
        else
        {
            List<GetCohortTopRiskStudentsByComponentResponse> result = await _context.ErasCalculationsByPoll
                            .Where(V => V.PollUuid == PollUuid && V.CohortId == CohortId && V.PollVersion != pollVersion)
                            .GroupBy(V => new { V.PollInstanceId, V.StudentName, V.StudentId })
                            .Select(G => new GetCohortTopRiskStudentsByComponentResponse
                            {
                                StudentId = G.Key.StudentId,
                                StudentName = G.Key.StudentName,
                                AnswerAverage = Math.Round((decimal)G.Average(V => V.AnswerRisk), 2),
                                RiskSum = G.Sum(V => V.AnswerRisk)
                            })
                            .OrderByDescending(G => G.RiskSum)
                            .ToListAsync();
            return result;
        }
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
