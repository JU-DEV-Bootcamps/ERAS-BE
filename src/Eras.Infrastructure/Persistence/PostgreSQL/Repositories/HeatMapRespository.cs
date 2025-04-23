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

        public async Task<
            IEnumerable<GetHeatMapByComponentsQueryResponse>
        > GetHeatMapDataByComponentsAsync(string PollUUID)
        {
            var query =
                from v in _context.Variables
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
                    AnswerRiskLevel = a.RiskLevel,
                };

            return await query
                .OrderBy(C => C.ComponentId)
                .ThenBy(V => V.VariableId)
                .ThenBy(A => A.AnswerText)
                .ToListAsync();
        }

        public async Task<
            IEnumerable<GetHeatMapByComponentsQueryResponse>
        > GetHeatMapDataByCohortAndDaysAsync(int? CohortId, int? Days)
        {
            var pollInstances = await GetPollInstancesByCohortIdAndLastDaysAsync(CohortId, Days);

            var pollInstanceIds = pollInstances.Select(Pi => Pi.Id).ToList();

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
                .OrderBy(C => C.ComponentId)
                .ThenBy(V => V.VariableId)
                .ThenBy(A => A.AnswerText)
                .ToListAsync();
        }

        internal async Task<IEnumerable<PollInstanceEntity>> GetPollInstancesByCohortIdAndLastDaysAsync(
            int? CohortId,
            int? Days
        )
        {
            IQueryable<PollInstanceEntity> query = _context
                .PollInstances.Include(Pi => Pi.Student)
                .Include(Pi => Pi.Answers);

            if (CohortId.HasValue && CohortId != 0)
            {
                query = query
                    .Join(
                        _context.StudentCohorts,
                        PollInstance => PollInstance.StudentId,
                        StudentCohort => StudentCohort.StudentId,
                        (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort = StudentCohort }
                    )
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

        public async Task<List<HeatMapBaseData>> GetHeatMapByPollUuidAndVariableIds(
            string PollUuid,
            List<int> VariableIds
        )
        {
            var heatmap = await (
                from pv in _context.PollVariables
                join v in _context.Variables on pv.VariableId equals v.Id
                join a in _context.Answers on pv.Id equals a.PollVariableId
                join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                join s in _context.Students on pi.StudentId equals s.Id
                join sc in _context.StudentCohorts on s.Id equals sc.StudentId
                where pi.Uuid == PollUuid && VariableIds.Contains(v.Id)
                group new
                {
                    v.Name,
                    a.AnswerText,
                    a.RiskLevel,
                } by v.Name into g
                select new HeatMapBaseData
                {
                    Name = g.Key,
                    Data = g.GroupBy(X => X.AnswerText)
                        .Select(G => new Serie
                        {
                            X = G.Key,
                            Y = (int)Math.Round(G.Average(X => X.RiskLevel)),
                            Count = G.Count(),
                        })
                        .ToList(),
                }
            ).ToListAsync();

            return heatmap;
        }

        public async Task<
            IEnumerable<GetHeatMapAnswersPercentageByVariableQueryResponse>
        > GetHeatMapAnswersPercentageByVariableAsync(string PollUUID)
        {
            if (string.IsNullOrWhiteSpace(PollUUID))
            {
                throw new ArgumentException(
                    "The Poll UUID can't be null or empty.",
                    nameof(PollUUID)
                );
            }

            try
            {
                // Calculate the total answers
                var totalAnswersByVariable = await _context
                    .Answers.Join(
                        _context.PollVariables,
                        A => A.PollVariableId,
                        Pv => Pv.Id,
                        (A, Pv) => new { a = A, pv = Pv }
                    )
                    .Join(
                        _context.Polls,
                        Apv => Apv.pv.PollId,
                        P => P.Id,
                        (Apv, P) =>
                            new
                            {
                                Apv.a,
                                Apv.pv,
                                P,
                            }
                    )
                    .Where(Apvp => Apvp.P.Uuid == PollUUID)
                    .GroupBy(Apvp => Apvp.a.PollVariableId)
                    .ToDictionaryAsync(Group => Group.Key, Group => Group.Count());

                // Calculate percentage
                var query = await _context
                    .Answers.Join(
                        _context.PollVariables,
                        A => A.PollVariableId,
                        Pv => Pv.Id,
                        (A, Pv) => new { a = A, pv = Pv }
                    )
                    .Join(
                        _context.Variables,
                        Apv => Apv.pv.VariableId,
                        V => V.Id,
                        (Apv, V) =>
                            new
                            {
                                Apv.a,
                                Apv.pv,
                                v = V,
                            }
                    )
                    .Join(
                        _context.Components,
                        Apv => Apv.v.ComponentId,
                        C => C.Id,
                        (Apv, C) =>
                            new
                            {
                                Apv.a,
                                Apv.pv,
                                Apv.v,
                                c = C,
                            }
                    )
                    .Join(
                        _context.Polls,
                        Apv => Apv.pv.PollId,
                        P => P.Id,
                        (Apv, P) =>
                            new
                            {
                                Apv.a,
                                Apv.v,
                                Apv.c,
                                p = P,
                            }
                    )
                    .Where(Apvp => Apvp.p.Uuid == PollUUID)
                    .GroupBy(Apvp => new
                    {
                        ComponentName = Apvp.c.Name,
                        PollVariableId = Apvp.a.PollVariableId,
                        VariableName = Apvp.v.Name,
                        Apvp.a.AnswerText,
                    })
                    .Select(Group => new
                    {
                        Group.Key.ComponentName,
                        Group.Key.PollVariableId,
                        Group.Key.VariableName,
                        Group.Key.AnswerText,
                        AnswerCount = Group.Count(),
                    })
                    .OrderBy(Result => Result.PollVariableId)
                    .ThenByDescending(Result => Result.AnswerCount)
                    .ToListAsync();

                // Calculate answer percentage
                var results = query.Select(
                    Item => new GetHeatMapAnswersPercentageByVariableQueryResponse
                    {
                        ComponentName = Item.ComponentName,
                        PollVariableId = Item.PollVariableId,
                        Name = Item.VariableName,
                        AnswerText = Item.AnswerText,
                        AnswerCount = Item.AnswerCount,
                        Percentage = Math.Round(
                            (Item.AnswerCount * 100.0)
                                / totalAnswersByVariable[Item.PollVariableId],
                            2
                        ),
                    }
                );

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"There is an error while running the query for the poll: {PollUUID} with the exception {ex}"
                );
                throw new ApplicationException("Error Processing the data for the heatmap.", ex);
            }
        }
    }
}
