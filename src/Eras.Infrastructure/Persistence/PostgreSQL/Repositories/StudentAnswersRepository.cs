using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentAnswersRepository : IStudentAnswersRepository
    {
        public readonly AppDbContext _context;

        public StudentAnswersRepository(AppDbContext Context)
        {
            _context = Context;
        }
        public Task<PagedResult<StudentAnswer>> GetStudentAnswersPagedAsync(int StudentId, int PollId, int Page, int PageSize)
        {
            var studentAnswers = _context.ErasCalculationsByPoll
                .Where(e => e.StudentId == StudentId && e.PollId == PollId)
                .Select(e => new StudentAnswer
                {
                    Variable = e.Question,
                    Position = e.PollVariableId,
                    Component = e.ComponentName,
                    Answer = e.AnswerText,
                    Score = e.AnswerRisk
                })
                .Skip((Page - 1) * PageSize)
                .Take(PageSize);

            var totalCount = _context.ErasCalculationsByPoll
                .Count(e => e.StudentId == StudentId && e.PollId == PollId);

            return Task.FromResult(new PagedResult<StudentAnswer>(totalCount, studentAnswers.ToList()));
        }
        public Task<List<StudentAnswer>> GetStudentAnswersAsync(int StudentId, int PollId)
        {
            var studentAnswers = from a in _context.Answers
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
            return Task.FromResult(studentAnswers.ToList());
        }

        public Task<StudentAnswer> UpdateAsync(StudentAnswer Entity) => throw new NotImplementedException();
    }
}
