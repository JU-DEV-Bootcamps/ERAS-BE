using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class ComponentsAvgRepository : IComponentsAvgRepository
    {
        private readonly AppDbContext _context;
        
        public ComponentsAvgRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ComponentsAvg>> ComponentsAvgByStudent(int studentId, int pollId)
        {
            List<ComponentsAvg> result = await _context.ErasCalculationsByPoll
                                .Where(v => v.PollInstanceId == studentId && v.PollId == pollId)
                                .GroupBy(v => new { v.PollId, v.ComponentId, v.ComponentName })
                                .Select(g => new ComponentsAvg
                                {
                                    PollId = g.Key.PollId,
                                    ComponentId = g.Key.ComponentId,
                                    Name = g.Key.ComponentName,
                                    ComponentAvg = (float)g.Average(v => v.RiskSum)
                                })
                                .ToListAsync();
            return result;
        }
    }
}
