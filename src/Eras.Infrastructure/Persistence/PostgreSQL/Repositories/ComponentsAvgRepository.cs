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
            List<ComponentsAvg> result = await _context.Components
                                    .Join(_context.Variables,
                                        c => c.Id,
                                        v => v.ComponentId,
                                        (c, v) => new { c, v })
                                    .Join(_context.PollVariables,
                                        cv => cv.v.Id,
                                        pv => pv.VariableId,
                                        (cv, pv) => new { cv.c, cv.v, pv })
                                    .Join(_context.Answers,
                                        cvpv => cvpv.pv.VariableId,
                                        a => a.PollVariableId,
                                        (cvpv, a) => new { cvpv.c, cvpv.pv, a })
                                    .Join(_context.PollInstances,
                                        temp => temp.a.PollInstanceId,
                                        pi => pi.Id,
                                        (temp, pi) => new { temp.c, temp.pv, temp.a, pi })
                                    .Where(x => x.pi.StudentId == studentId && x.pv.PollId == pollId)
                                    .GroupBy(x => new { x.c.Id, x.c.Name })
                                    .Select(g => new ComponentsAvg
                                    {
                                        Id = g.Key.Id,
                                        Name = g.Key.Name,
                                        ComponentAvg = (float)g.Average(x => x.a.RiskLevel)
                                    })
                                    .ToListAsync();
            return result;
        }
    }
}
