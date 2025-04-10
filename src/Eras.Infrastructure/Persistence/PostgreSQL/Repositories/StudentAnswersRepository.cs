using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentAnswersRepository(AppDbContext Context) : IStudentAnswersRepository
    {
        public readonly AppDbContext _context = Context;

        public async Task<List<StudentAnswer>> GetStudentAnswersAsync(int StudentId, int PollId)
        {
            IQueryable<StudentAnswer> studentAnswers = from a in _context.Answers
                                 join pi2 in _context.PollInstances on a.PollInstanceId equals pi2.Id
                                 join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                                 join v in _context.Variables on pv.VariableId equals v.Id
                                 join c in _context.Components on v.ComponentId equals c.Id
                                 where pv.PollId == PollId && pi2.StudentId == StudentId
                                 select new StudentAnswer
                                 {
                                     Variable = v.Name,
                                     Position = a.PollVariableId,
                                     Component = c.Name,
                                     Answer = a.AnswerText,
                                     Score = a.RiskLevel
                                 };
            return [.. studentAnswers];
        }
    }
}
