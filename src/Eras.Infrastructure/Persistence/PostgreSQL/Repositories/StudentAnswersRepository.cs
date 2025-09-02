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
            var distinctQuestions = _context.ErasCalculationsByPoll
                .Where(E => E.StudentId == StudentId && E.PollId == PollId)
                .Select(E => new { E.Question, E.Position, E.ComponentName })
                .Distinct()
                .OrderBy(q => q.Position);

            var totalCount = distinctQuestions.Count();

            var pagedQuestions = distinctQuestions
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var studentAnswers = new List<StudentAnswer>();
            foreach (var q in pagedQuestions)
            {
                var answer = _context.ErasCalculationsByPoll
                    .Where(E => E.StudentId == StudentId && E.PollId == PollId && 
                               E.Question == q.Question && E.Position == q.Position && 
                               E.ComponentName == q.ComponentName)
                    .Select(E => new StudentAnswer
                    {
                        Variable = E.Question,
                        Position = E.Position,
                        Component = E.ComponentName,
                        Answer = E.AnswerText,
                        Score = E.AnswerRisk
                    })
                    .First();
                
                studentAnswers.Add(answer);
            }

            return Task.FromResult(new PagedResult<StudentAnswer>(totalCount, studentAnswers));
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
