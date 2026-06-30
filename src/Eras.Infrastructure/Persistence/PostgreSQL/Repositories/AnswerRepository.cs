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

    public async Task<List<Answer>> GetByPollInstanceIdAsync(int Id)
    {
        List<Answer> answers = await _context.Answers
            .Where(Answ => Answ.PollInstanceId.Equals(Id))
            .Select(Ans => Ans.ToDomain())
            .ToListAsync();

        return answers;
    }
    public async Task<List<Answer>> GetByPollInstanceAnswerAndPollVariableAsync(int PollVariableId,
        int PollInstanceId, string AnswerText)
    {
        List<AnswerEntity> answers = await _context.Answers
            .Where(Answ => Answ.PollInstanceId == PollInstanceId && 
            Answ.AnswerText.Equals(AnswerText) && Answ.PollVariableId == PollVariableId).ToListAsync();
        return answers.Select(Answ => Answ.ToDomain()).ToList();
    }

    public async Task<int?> GetAnswerIdByPollInstanceAndVariableAsync(int PollVariableId, int PollInstanceId)
    {
        AnswerEntity? answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.PollVariableId == PollVariableId && 
                                    a.PollInstanceId == PollInstanceId);
        return answer?.Id;
    }

    public async Task UpdateAnswerTextAsync(int Id, string AnswerText, decimal RiskLevel)
    {
        await _context.Answers
            .Where(a => a.Id == Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(a => a.AnswerText, AnswerText)
                .SetProperty(a => a.RiskLevel, RiskLevel));
    }
}
