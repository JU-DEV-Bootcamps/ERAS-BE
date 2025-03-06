using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class AnswerRepository(AppDbContext context) : BaseRepository<Answer, AnswerEntity>
        (context, AnswerMapper.ToDomain, AnswerMapper.ToPersistence), IAnswerRepository
    {
        public async Task<List<Answer>?> GetByStudentIdAsync(string uuid)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student => student.Uuid == uuid);
            if (student == null) return null;
            var pollsOfStudent = await _context.PollInstances.Where(pollInstance => pollInstance.StudentId.Equals(student.Id)).ToListAsync();
            var lastPoll = pollsOfStudent.OrderByDescending(poll => poll.FinishedAt).FirstOrDefault();
            //If no answers found returns null
            if (lastPoll == null) return null;

            var answers = await _context.Answers
                .Where(answer => answer.PollInstanceId.Equals(lastPoll.Id))
                .ToListAsync();
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
        public async Task SaveManyAnswersAsync(List<Answer> answers)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var ans in answers)
                {
                    await _context.Answers.AddAsync(ans.ToPersistence());
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error al guardar los registros: {ex.Message}");
            }
        }
    }
}
