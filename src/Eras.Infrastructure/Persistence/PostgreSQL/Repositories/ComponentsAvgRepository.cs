using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ComponentsAvgRepository : IComponentsAvgRepository
    {
        private readonly AppDbContext _context;
        private readonly IAnswerRiskValidator _answerRiskValidator;

        public ComponentsAvgRepository(AppDbContext Context, IAnswerRiskValidator validator)
        {
            _context = Context;
            _answerRiskValidator = validator;
        }

        public async Task<List<ComponentsAvg>> ComponentsAvgByStudent(int StudentId, int PollId)
        {
            var rows = await _context.ErasCalculationsByPoll
                .Where(V => V.StudentId == StudentId && V.PollId == PollId)
                .Select(V => new 
                {
                    V.PollId,
                    V.ComponentId,
                    V.ComponentName,
                    V.AnswerRisk,
                    V.AnswerText
                })
                .ToListAsync();

            List<ComponentsAvg> result = rows
                .GroupBy(V => new { V.PollId, V.ComponentId, V.ComponentName })
                .Select(G => new ComponentsAvg
                {
                    PollId = G.Key.PollId,
                    ComponentId = G.Key.ComponentId,
                    Name = G.Key.ComponentName,
                    ComponentAvg = (float) Math.Round(
                        G.Where(V => 
                            _answerRiskValidator.IsValidAnswer(V.AnswerText))
                        .Average(V => (double)V.AnswerRisk), 2)
                })
                .ToList();

            return result;
        }
    }
}
