using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class AnswerRepository : BaseRepository<Answer, AnswerEntity>, IAnswerRepository
    {
        public AnswerRepository(AppDbContext context)
            : base(context, AnswerMapper.ToDomain, AnswerMapper.ToPersistence)
        {
        }
        public async Task<List<Answer>?> GetByStudentIdAsync(string uuid)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student => student.Uuid == uuid);
            if (student == null) return null;
            var pollsOfStudent = await _context.PollInstances.Where(pollInstance => pollInstance.StudentId.Equals(student.Id)).ToListAsync();
            var answers = new List<AnswerEntity>();
            foreach(var poll in pollsOfStudent)
            {
                var pollAnswers = await _context.Answers.Where(answer => answer.PollInstanceId.Equals(poll.Id)).ToListAsync();
                answers.AddRange(pollAnswers);
            }
            //TODO: Verify performance for this query
            // var answers = await _context.Answers
            //   .Where(answer => answer.PollInstance.StudentId.Equals(id)).ToListAsync();
            var domainAnswers = new List<Answer>();
            foreach(var answer in answers)
            {
                domainAnswers.Add(answer.ToDomain());
            }
            return domainAnswers;
        }

        public async Task<List<Answer>?> GetByPollInstanceIdAsync(string uuid)
        {
            var answers = await _context.Answers
              .Where(answer => answer.PollInstanceId.Equals(uuid)).ToListAsync();
            var domainAnswers = new List<Answer>();
            foreach(var answer in answers)
            {
                domainAnswers.Add(answer.ToDomain());
            }
            return domainAnswers;
        }
    }
}
