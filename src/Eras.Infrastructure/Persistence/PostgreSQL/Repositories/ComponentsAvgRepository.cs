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
        
        public ComponentsAvgRepository(AppDbContext Context)
        {
            _context = Context;
        }

        public async Task<List<ComponentsAvg>> ComponentsAvgByStudent(int StudentId, int PollId)
        {
            List<ComponentsAvg> result = await _context.Components
                                    .Join(_context.Variables,
                                        C => C.Id,
                                        V => V.ComponentId,
                                        (C, V) => new { c= C, v = V })
                                    .Join(_context.PollVariables,
                                        Cv => Cv.v.Id,
                                        Pv => Pv.VariableId,
                                        (Cv, Pv) => new { Cv.c, Cv.v, pv= Pv })
                                    .Join(_context.Answers,
                                        Cvpv => Cvpv.pv.VariableId,
                                        A => A.PollVariableId,
                                        (Cvpv, A) => new { Cvpv.c, Cvpv.pv, a=A })
                                    .Join(_context.PollInstances,
                                        Temp => Temp.a.PollInstanceId,
                                        Pi => Pi.Id,
                                        (Temp, Pi) => new { Temp.c, Temp.pv, Temp.a, Pi })
                                    .Where(X => X.Pi.StudentId == StudentId && X.pv.PollId == PollId)
                                    .GroupBy(X => new { X.c.Id, X.c.Name })
                                    .Select(G => new ComponentsAvg
                                    {
                                        PollId = PollId,
                                        ComponentId = G.Key.Id,
                                        Name = G.Key.Name,
                                        ComponentAvg = (float)G.Average(X => X.a.RiskLevel)
                                    })
                                    .ToListAsync();
            return result;
        }
    }
}
