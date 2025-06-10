using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Poll;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Calculations;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollCohortRepository : BaseRepository<Poll, PollEntity>, IPollCohortRepository
    {
        public PollCohortRepository(AppDbContext Context)
            : base(Context, Mappers.PollMapper.ToDomain, Mappers.PollMapper.ToPersistence) { }

        public async Task<List<Poll>> GetPollsByCohortIdAsync(int CohortId)
        {
            var polls = await _context
                .Cohorts.Where(C => C.Id == CohortId)
                .Join(
                    _context.StudentCohorts,
                    Cohort => Cohort.Id,
                    StudentCohort => StudentCohort.CohortId,
                    (Cohort, StudentCohort) => StudentCohort
                )
                .Join(
                    _context.Students,
                    Sc => Sc.StudentId,
                    Student => Student.Id,
                    (Sc, Student) => Student
                )
                .Join(
                    _context.PollInstances,
                    Student => Student.Id,
                    PollInstance => PollInstance.StudentId,
                    (Student, PollInstance) => PollInstance
                )
                .Join(
                    _context.Answers,
                    PollInstance => PollInstance.Id,
                    Answer => Answer.PollInstanceId,
                    (PollInstance, Answer) => Answer
                )
                .Join(
                    _context.PollVariables,
                    Answer => Answer.PollVariableId,
                    PollVariable => PollVariable.Id,
                    (Answer, PollVariable) => PollVariable
                )
                .Join(
                    _context.Polls,
                    PollVariable => PollVariable.PollId,
                    Poll => Poll.Id,
                    (PollVariable, Poll) => Poll
                )
                .Distinct()
                .ToListAsync();
            return polls.Select(P => P.ToDomain()).ToList();
        }

        public async Task<List<PollVariableDto>> GetPollVariablesAsync(int PollId, int CohortId)
        {
            var query =
                from pv in _context.PollVariables
                join v in _context.Variables on pv.VariableId equals v.Id
                join a in _context.Answers on pv.Id equals a.PollVariableId
                join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                join sc in _context.StudentCohorts on pi.StudentId equals sc.StudentId
                join c in _context.Cohorts on sc.CohortId equals c.Id
                where pv.PollId == PollId && c.Id == CohortId
                orderby pv.PollId, v.Id
                select new PollVariableDto
                {
                    PollId = pv.PollId,
                    VariableId = v.Id,
                    VariableName = v.Name,
                };

            return await query.Distinct().ToListAsync();
        }

        public async Task<List<GetCohortComponentsByPollResponse>> GetCohortComponentsByPoll(string PollUuid, bool LastVersion)
        {

            int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

            if (LastVersion)
            {
                var query = await _context.ErasCalculationsByPoll
                        .Where(V => V.PollUuid == PollUuid && V.PollVersion == pollVersion)
                        .Select(V => new GetCohortComponentsByPollResponse
                        {
                            CohortId = V.CohortId,
                            CohortName = V.CohortName,
                            ComponentName = V.ComponentName,
                            AverageRiskByCohortComponent = Math.Round(V.AverageRiskByCohortComponent, 2)
                        })
                        .Distinct()
                        .ToListAsync();
                return query;
            }
            else
            {
                var query = await _context.ErasCalculationsByPoll
                        .Where(V => V.PollUuid == PollUuid && V.PollVersion != pollVersion)
                        .Select(V => new GetCohortComponentsByPollResponse
                        {
                            CohortId = V.CohortId,
                            CohortName = V.CohortName,
                            ComponentName = V.ComponentName,
                            AverageRiskByCohortComponent = Math.Round(V.AverageRiskByCohortComponent, 2)
                        })
                        .Distinct()
                        .ToListAsync();
                return query;
            }
        }

        public async Task<List<GetCohortStudentsRiskByPollResponse>> GetCohortStudentsRiskByPoll(string PollUuid, int CohortId)
        {
            var query = await _context.ErasCalculationsByPoll
               .Where(V => V.PollUuid == PollUuid && V.CohortId == CohortId)
               .OrderByDescending(V => V.PollInstanceRiskSum)
               .Select(V => new GetCohortStudentsRiskByPollResponse
               {
                   PollInstanceId = V.PollInstanceId,
                   StudentName = V.StudentName,
                   PollInstanceRiskSum = V.PollInstanceRiskSum
               })
               .ToListAsync();

            var distinctResult = query
                .DistinctBy(X => X.PollInstanceId)
                .ToList();

            return distinctResult;
        }
    }
}
