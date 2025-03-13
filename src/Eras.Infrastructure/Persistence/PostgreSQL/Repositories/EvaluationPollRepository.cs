using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    internal class EvaluationPollRepository(AppDbContext context) : BaseRepository<Evaluation,EvaluationPollJoin>(context, EvaluationPollMapper.ToDomain, EvaluationPollMapper.ToPersistence), IEvaluationPollRepository
    {
        public void GetAllPollsPollInstances(int studentId, int pollId)
        {
            List<EvaluationPollJoin> result = _context.EvaluationPolls.
                Include(ep => ep.Poll)
            //     Include(cs => cs.Student).
            //     ThenInclude(s => s.PollInstances).
            //     ThenInclude(p => p.Answers).
            //     ThenInclude(a => a.PollVariable).
            //     ToListAsync();
            // var cohortsDomain = cohorts.Select(c => (
            //     Student: c.ToJoinDomain(),
            //     PollInstances: c.Student.PollInstances.Select(p => p.ToSummaryDomain()).ToList())
                 .ToList();
        }
    }
}
