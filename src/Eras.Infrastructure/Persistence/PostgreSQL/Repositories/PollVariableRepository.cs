using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollVariableRepository : BaseRepository<Variable, PollVariableJoin>, IPollVariableRepository
    {
        public PollVariableRepository(AppDbContext context )
            : base(context, PollVariableMapper.ToDomain, PollVariableMapper.ToPersistenceVariable)
        {

        }
        public async Task<Variable?> GetByPollIdAndVariableIdAsync(int pollId, int variableId)
        {
            var pollVariable = await _context.PollVariables
                .FirstOrDefaultAsync(pollVar => pollVar.PollId == pollId && pollVar.VariableId == variableId);

            return pollVariable?.ToDomain();
        }

        public async Task<List<(Answer Answer, Variable Variable, int StudentId)>> GetByPollUuidAsync(string pollUuid)
        {
            //Assuming the pollUuid and pollInstance Uuid are the same. This query uses the pollIntanceUuid
            var answers = await (from poll in _context.PollInstances
                                 join pollVariable in _context.PollVariables on poll.Id equals pollVariable.PollId
                                 join answer in _context.Answers on pollVariable.Id equals answer.PollVariableId
                                 join variable in _context.Variables on pollVariable.VariableId equals variable.Id
                                 where poll.Uuid == pollUuid
                                 select new { Answer = answer.ToDomain(), Variable = variable.ToDomain(), studentId = poll.StudentId }
                                 ).ToListAsync();
            List<(Answer, Variable, int)> values = [];
            foreach(var answer in answers)
            {
                values.Add((answer.Answer, answer.Variable, answer.studentId));
            }
            return values;
        }
    }
}
