using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

[ExcludeFromCodeCoverage]
public class AnswerRepository(AppDbContext Context) : BaseRepository<Answer, AnswerEntity>
    (Context, AnswerMapper.ToDomain, AnswerMapper.ToPersistence), IAnswerRepository
{
    public async Task<List<Answer>> GetByStudentIdAsync(string Uuid)
    {
        StudentEntity? student = await _context.Students.FirstOrDefaultAsync(Student => Student.Uuid == Uuid);
        if (student == null) return [];
        List<PollInstanceEntity> pollsOfStudent = await _context.PollInstances.Where(PollInstance => PollInstance.StudentId.Equals(student.Id)).ToListAsync();
        PollInstanceEntity? lastPoll = pollsOfStudent.OrderByDescending(Poll => Poll.FinishedAt).FirstOrDefault();
        if (lastPoll == null) return [];
        List<AnswerEntity> answers = await _context.Answers
            .Where(Ans => Ans.PollInstanceId.Equals(lastPoll.Id))
            .ToListAsync();
        var domainAnswers = new List<Answer>();
        foreach (AnswerEntity answer in answers)
        {
            domainAnswers.Add(answer.ToDomain());
        }
        return domainAnswers;
    }

    public async Task<List<Answer>> GetByPollInstanceIdAsync(string Uuid)
    {
        List<AnswerEntity> answers = await _context.Answers
            .Where(Answ => Answ.PollInstanceId.Equals(Uuid)).ToListAsync();
        var domainAnswers = new List<Answer>();
        foreach (AnswerEntity answer in answers)
        {
            domainAnswers.Add(answer.ToDomain());
        }
        return domainAnswers;
    }
    public async Task SaveManyAnswersAsync(List<Answer> Answers)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        foreach (Answer ans in Answers)
        {
            try
            {
                await _context.Answers.AddAsync(ans.ToPersistence());
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine($"Error storing answer: {ex.Message}");
            }
            await transaction.CommitAsync();
        }
    }

    public async Task<List<Answer>> GetByPollInstanceAnswerAndPollVariableAsync(int PollVariableId,
        int PollInstanceId, string AnswerText)
    {
        List<AnswerEntity> answers = await _context.Answers
            .Where(Answ => Answ.PollInstanceId == PollInstanceId && 
            Answ.AnswerText.Equals(AnswerText) && Answ.PollVariableId == PollVariableId).ToListAsync();
        return answers.Select(Answ => Answ.ToDomain()).ToList();
    }
}
