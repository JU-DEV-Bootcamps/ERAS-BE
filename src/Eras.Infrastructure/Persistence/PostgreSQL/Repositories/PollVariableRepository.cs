using System.Linq;
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

        public async Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string pollUuid, int varibaleId)
        {
            //Assuming the pollUuid and pollInstance Uuid are the same. This query uses the pollIntanceUuid
            var answers = await (from poll in _context.PollInstances
                                 join student in _context.Students on poll.StudentId equals student.Id
                                 join pollVariable in _context.PollVariables on poll.Id equals pollVariable.PollId
                                 join answer in _context.Answers on pollVariable.Id equals answer.PollVariableId
                                 join variable in _context.Variables on pollVariable.VariableId equals variable.Id
                                 where poll.Uuid == pollUuid
                                 where variable.Id == varibaleId
                                 where answer.PollVariableId == varibaleId
                                 select new { Answer = answer.ToDomain(), Variable = variable.ToDomain(), Student = student.ToDomain() }
                                 ).ToListAsync();
            return [.. answers.Select(a => (a.Answer, a.Variable, a.Student))];
        }
    }
}
