using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollCohortRepository : BaseRepository<Poll, PollEntity>, IPollCohortRepository
    {

        public PollCohortRepository(AppDbContext context) 
            : base(context, Mappers.PollMapper.ToDomain, Mappers.PollMapper.ToPersistence)
        {
        }

        public async  Task<List<Poll>> GetPollsByCohortIdAsync(int cohortId)
        {
            var polls = await _context.Cohorts
                .Where(c => c.Id == cohortId)
                .Join(_context.StudentCohorts,
                    cohort => cohort.Id,
                    studentCohort => studentCohort.CohortId,
                    (cohort, studentCohort) => studentCohort)
                .Join(_context.Students,
                    sc => sc.StudentId,
                    student => student.Id,
                    (sc, student) => student)
                .Join(_context.PollInstances,
                    student => student.Id,
                    pollInstance => pollInstance.StudentId,
                    (student, pollInstance) => pollInstance)
                .Join(_context.Answers,
                    pollInstance => pollInstance.Id,
                    answer => answer.PollInstanceId,
                    (pollInstance, answer) => answer)
                .Join(_context.PollVariables,
                    answer => answer.PollVariableId,
                    pollVariable => pollVariable.Id,
                    (answer, pollVariable) => pollVariable)
                .Join(_context.Polls,
                    pollVariable => pollVariable.PollId,
                    poll => poll.Id,
                    (pollVariable, poll) => poll)
                .Distinct()
                .ToListAsync();
            return polls.Select(p => p.ToDomain()).ToList();
        }
    }
}
