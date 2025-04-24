using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.HeatMap;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class HeatMapRespository : IHeatMapRepository
    {
        protected readonly AppDbContext _context;

        public HeatMapRespository(AppDbContext Context)
        {
            _context = Context;
        }

        public async Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> 
            GetHeatMapDataByComponentsAsync(string PollUUID)
        {
            var query = from v in _context.Variables
                        join c in _context.Components on v.ComponentId equals c.Id
                        join pv in _context.Set<PollVariableJoin>() on v.Id equals pv.VariableId
                        join a in _context.Answers on pv.Id equals a.PollVariableId
                        join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                        where pi.Uuid == PollUUID
                        select new GetHeatMapByComponentsQueryResponse
                        {
                            ComponentId = c.Id,
                            ComponentName = c.Name,
                            VariableId = v.Id,
                            VariableName = v.Name,
                            AnswerText = a.AnswerText,
                            AnswerRiskLevel = a.RiskLevel
                        };

            return await query.OrderBy(C => C.ComponentId)
                              .ThenBy(V => V.VariableId)
                              .ThenBy(A => A.AnswerText)
                              .ToListAsync();
        }

        public async Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> 
            GetHeatMapDataByCohortAndDaysAsync(int? CohortId, int? Days)
        {
            var pollInstances = await GetPollInstancesByCohortIdAndLastDaysAsync(CohortId, Days);

            var pollInstanceIds = pollInstances.Select(Pi => Pi.Id).ToList();

            var query = from v in _context.Variables
                        join c in _context.Components on v.ComponentId equals c.Id
                        join pv in _context.Set<PollVariableJoin>() on v.Id equals pv.VariableId
                        join a in _context.Answers on pv.Id equals a.PollVariableId
                        join pi in _context.PollInstances.AsEnumerable() on a.PollInstanceId equals pi.Id
                        where pollInstanceIds.Contains(pi.Id)
                        select new GetHeatMapByComponentsQueryResponse
                        {
                            ComponentId = c.Id,
                            ComponentName = c.Name,
                            VariableId = v.Id,
                            VariableName = v.Name,
                            AnswerText = a.AnswerText,
                            AnswerRiskLevel = a.RiskLevel
                        };

            return await query.OrderBy(C => C.ComponentId)
                              .ThenBy(V => V.VariableId)
                              .ThenBy(A => A.AnswerText)
                              .ToListAsync();
        }

        internal async Task<IEnumerable<PollInstanceEntity>> 
            GetPollInstancesByCohortIdAndLastDaysAsync(int? CohortId, int? Days)
        {
            IQueryable<PollInstanceEntity> query = _context.PollInstances
                .Include(Pi => Pi.Student)
                .Include(Pi => Pi.Answers);

            if (CohortId.HasValue && CohortId != 0)
            {
                query = query
                    .Join(_context.StudentCohorts,
                        PollInstance => PollInstance.StudentId,
                        StudentCohort => StudentCohort.StudentId,
                        (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort=StudentCohort })
                    .Where(Joined => Joined.studentCohort.CohortId == CohortId.Value)
                    .Select(Joined => Joined.pollInstance);
            }

            if (Days.HasValue && Days != 0)
            {
                var dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
                query = query.Where(Pi => Pi.FinishedAt >= dateLimit);
            }

            return await query.Distinct().ToListAsync();
        }
    }
}

