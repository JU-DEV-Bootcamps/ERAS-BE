using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ComponentsAvgRepository : IComponentsAvgRepository
    {
        private readonly AppDbContext _context;

        public ComponentsAvgRepository(AppDbContext Context)
        {
            _context = Context;
        }

        public async Task<List<ComponentsAvg>> ComponentsAvgByStudent(int StudentId, int PollId)
        {
            List<ComponentsAvg> result = await _context.ErasCalculationsByPoll
                                .Where(V => V.PollInstanceId == StudentId && V.PollId == PollId)
                                .GroupBy(V => new { V.PollId, V.ComponentId, V.ComponentName })
                                .Select(G => new ComponentsAvg
                                {
                                    PollId = G.Key.PollId,
                                    ComponentId = G.Key.ComponentId,
                                    Name = G.Key.ComponentName,
                                    ComponentAvg = (float) G.Average(V => V.AnswerRisk)
                                })
                                .ToListAsync();
            return result;
        }
    }
}
