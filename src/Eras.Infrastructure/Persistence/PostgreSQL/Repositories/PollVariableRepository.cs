using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollVariableRepository : BaseRepository<Variable, PollVariableJoin>, IPollVariableRepository
    {
        public PollVariableRepository(AppDbContext context)
            : base(context, PollVariableMapper.ToDomain, PollVariableMapper.ToPersistenceVariable)
        {

        }
        public async Task<Variable?> GetByPollIdAndVariableIdAsync(int pollId, int variableId)
        {
            var pollVariable = await _context.PollVariables
                .FirstOrDefaultAsync(pollVar => pollVar.PollId == pollId && pollVar.VariableId == variableId);

            return pollVariable?.ToDomain();
        }

        public async Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string pollUuid, int variableId)
        {
            //Assuming the pollUuid and pollInstance Uuid are the same. This query uses the pollIntanceUuid
            var answers = await (from s in _context.Students
                     join pi in _context.PollInstances on s.Id equals pi.StudentId
                     join a in _context.Answers on pi.Id equals a.PollInstanceId
                     join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                     join v in _context.Variables on pv.VariableId equals v.Id
                     join c in _context.Components on v.ComponentId equals c.Id
                     where pi.Uuid == pollUuid && v.Id == variableId
                     select new { Answer = a.ToDomain(), Variable = v.ToDomain(), Student = s.ToDomain() }
                     ).ToListAsync();
            return [.. answers.Select(a => (a.Answer, a.Variable, a.Student))];
        }
        public async Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string pollUuid, string variableIds)
        {
            var variableIdsArray = variableIds.Split(',').Select(int.Parse).ToArray();

            var answers = await (from s in _context.Students
                     join pi in _context.PollInstances on s.Id equals pi.StudentId
                     join a in _context.Answers on pi.Id equals a.PollInstanceId
                     join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                     join v in _context.Variables on pv.VariableId equals v.Id
                     join c in _context.Components on v.ComponentId equals c.Id
                     where pi.Uuid == pollUuid && variableIdsArray.Contains(v.Id)
                     select new { Answer = a.ToDomain(), Variable = v.ToDomain(), Student = s.ToDomain() }
                    ).ToListAsync();

            var groupedResults = answers
                .GroupBy(a => a.Student.Name)
                .Select(group =>
                {
                    var averageRisk = group.Average(g => g.Answer.RiskLevel);
                    var firstAnswer = group.First();
                    return (Answer: firstAnswer.Answer, Variable: firstAnswer.Variable, Student: firstAnswer.Student, AverageRisk: averageRisk);
                })
                .ToList();

            return groupedResults.Select(g => (g.Answer, g.Variable, g.Student)).ToList();
        }
    }
}
