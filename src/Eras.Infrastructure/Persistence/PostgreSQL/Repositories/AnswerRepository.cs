using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class AnswerRepository(AppDbContext Context) : BaseRepository<Answer, AnswerEntity>
        (Context, AnswerMapper.ToDomain, AnswerMapper.ToPersistence), IAnswerRepository
    {
        public async Task<List<Answer>?> GetByStudentIdAsync(string Uuid)
        {
            var student = await _context.Students.FirstOrDefaultAsync(Student => Student.Uuid == Uuid);
            if (student == null) return null;
            var pollsOfStudent = await _context.PollInstances.Where(PollInstance => PollInstance.StudentId.Equals(student.Id)).ToListAsync();
            var lastPoll = pollsOfStudent.OrderByDescending(Poll => Poll.FinishedAt).FirstOrDefault();
            //If no answers found returns null
            if (lastPoll == null) return null;

            var answers = await _context.Answers
                .Where(Answer => Answer.PollInstanceId.Equals(lastPoll.Id))
                .ToListAsync();
            var domainAnswers = new List<Answer>();
            foreach(var answer in answers)
            {
                domainAnswers.Add(answer.ToDomain());
            }
            return domainAnswers;
        }

        public async Task<List<Answer>?> GetByPollInstanceIdAsync(string Uuid)
        {
            var answers = await _context.Answers
                .Where(Answer => Answer.PollInstanceId.Equals(Uuid)).ToListAsync();
            var domainAnswers = new List<Answer>();
            foreach(var answer in answers)
            {
                domainAnswers.Add(answer.ToDomain());
            }
            return domainAnswers;
        }
        public async Task SaveManyAnswersAsync(List<Answer> Answers)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var ans in Answers)
                {
                    await _context.Answers.AddAsync(ans.ToPersistence());
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error storing answer: {ex.Message}");
            }
        }
    }
}
