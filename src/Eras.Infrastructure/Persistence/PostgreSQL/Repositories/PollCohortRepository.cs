using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Poll;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollCohortRepository : BaseRepository<Poll, PollEntity>, IPollCohortRepository
    {
        public PollCohortRepository(AppDbContext context)
            : base(context, Mappers.PollMapper.ToDomain, Mappers.PollMapper.ToPersistence) { }

        public async Task<List<Poll>> GetPollsByCohortIdAsync(int cohortId)
        {
            var polls = await _context
                .Cohorts.Where(c => c.Id == cohortId)
                .Join(
                    _context.StudentCohorts,
                    cohort => cohort.Id,
                    studentCohort => studentCohort.CohortId,
                    (cohort, studentCohort) => studentCohort
                )
                .Join(
                    _context.Students,
                    sc => sc.StudentId,
                    student => student.Id,
                    (sc, student) => student
                )
                .Join(
                    _context.PollInstances,
                    student => student.Id,
                    pollInstance => pollInstance.StudentId,
                    (student, pollInstance) => pollInstance
                )
                .Join(
                    _context.Answers,
                    pollInstance => pollInstance.Id,
                    answer => answer.PollInstanceId,
                    (pollInstance, answer) => answer
                )
                .Join(
                    _context.PollVariables,
                    answer => answer.PollVariableId,
                    pollVariable => pollVariable.Id,
                    (answer, pollVariable) => pollVariable
                )
                .Join(
                    _context.Polls,
                    pollVariable => pollVariable.PollId,
                    poll => poll.Id,
                    (pollVariable, poll) => poll
                )
                .Distinct()
                .ToListAsync();
            return polls.Select(p => p.ToDomain()).ToList();
        }

        public async Task<List<PollVariableDto>> GetPollVariablesAsync(int pollId, int cohortId)
        {
            var query =
                from pv in _context.PollVariables
                join v in _context.Variables on pv.VariableId equals v.Id
                join a in _context.Answers on pv.Id equals a.PollVariableId
                join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                join sc in _context.StudentCohorts on pi.StudentId equals sc.StudentId
                join c in _context.Cohorts on sc.CohortId equals c.Id
                where pv.PollId == pollId && c.Id == cohortId
                orderby pv.PollId, v.Id
                select new PollVariableDto
                {
                    PollId = pv.PollId,
                    VariableId = v.Id,
                    VariableName = v.Name,
                };

            return await query.Distinct().ToListAsync();
        }
    }
}
