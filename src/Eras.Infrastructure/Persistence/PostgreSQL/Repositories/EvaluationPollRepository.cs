using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class EvaluationPollRepository(AppDbContext Context) : BaseRepository<Evaluation, EvaluationPollJoin>(Context, EvaluationPollMapper.ToDomain, EvaluationPollMapper.ToPersistence), IEvaluationPollRepository
    {
        public async Task<bool> ExistsAsync(int EvaluationId, int PollId)
        {
            return await _context.Set<EvaluationPollJoin>()
                .AnyAsync(EvaluationPoll => EvaluationPoll.EvaluationId == EvaluationId && EvaluationPoll.PollId == PollId);
        }
    }
}
