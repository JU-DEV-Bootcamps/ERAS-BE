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

        public HeatMapRespository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<
            IEnumerable<GetHeatMapByComponentsQueryResponse>
        > GetHeatMapDataByComponentsAsync(string pollUUID)
        {
            var query =
                from v in _context.Variables
                join c in _context.Components on v.ComponentId equals c.Id
                join pv in _context.Set<PollVariableJoin>() on v.Id equals pv.VariableId
                join a in _context.Answers on pv.Id equals a.PollVariableId
                join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                where pi.Uuid == pollUUID
                select new GetHeatMapByComponentsQueryResponse
                {
                    ComponentId = c.Id,
                    ComponentName = c.Name,
                    VariableId = v.Id,
                    VariableName = v.Name,
                    AnswerText = a.AnswerText,
                    AnswerRiskLevel = a.RiskLevel,
                };

            return await query
                .OrderBy(c => c.ComponentId)
                .ThenBy(v => v.VariableId)
                .ThenBy(a => a.AnswerText)
                .ToListAsync();
        }

        public async Task<
            IEnumerable<GetHeatMapByComponentsQueryResponse>
        > GetHeatMapDataByCohortAndDaysAsync(int? cohortId, int? days)
        {
            var pollInstances = await GetPollInstancesByCohortIdAndLastDays(cohortId, days);

            var pollInstanceIds = pollInstances.Select(pi => pi.Id).ToList();

            var query =
                from v in _context.Variables
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
                    AnswerRiskLevel = a.RiskLevel,
                };

            return await query
                .OrderBy(c => c.ComponentId)
                .ThenBy(v => v.VariableId)
                .ThenBy(a => a.AnswerText)
                .ToListAsync();
        }

        internal async Task<IEnumerable<PollInstanceEntity>> GetPollInstancesByCohortIdAndLastDays(
            int? cohortId,
            int? days
        )
        {
            IQueryable<PollInstanceEntity> query = _context
                .PollInstances.Include(pi => pi.Student)
                .Include(pi => pi.Answers);

            if (cohortId.HasValue && cohortId != 0)
            {
                query = query
                    .Join(
                        _context.StudentCohorts,
                        pollInstance => pollInstance.StudentId,
                        studentCohort => studentCohort.StudentId,
                        (pollInstance, studentCohort) => new { pollInstance, studentCohort }
                    )
                    .Where(joined => joined.studentCohort.CohortId == cohortId.Value)
                    .Select(joined => joined.pollInstance);
            }

            if (days.HasValue && days != 0)
            {
                var dateLimit = DateTime.UtcNow.AddDays(-days.Value);
                query = query.Where(pi => pi.FinishedAt >= dateLimit);
            }

            return await query.Distinct().ToListAsync();
        }

        public async Task<List<HeatMapBaseData>> GetHeatMapByPollUuidAndVariableIds(
            string pollUuid,
            List<int> variableIds
        )
        {
            var heatmap = await (
                from pv in _context.PollVariables
                join v in _context.Variables on pv.VariableId equals v.Id
                join a in _context.Answers on pv.Id equals a.PollVariableId
                join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                join s in _context.Students on pi.StudentId equals s.Id
                join sc in _context.StudentCohorts on s.Id equals sc.StudentId
                where pi.Uuid == pollUuid && variableIds.Contains(v.Id)
                group new
                {
                    v.Name,
                    a.AnswerText,
                    a.RiskLevel,
                } by v.Name into g
                select new HeatMapBaseData
                {
                    Name = g.Key,
                    Data = g.Select(x => new Serie { X = x.AnswerText, Y = x.RiskLevel }).ToList(),
                }
            ).ToListAsync();

            return heatmap;
        }
    }
}
