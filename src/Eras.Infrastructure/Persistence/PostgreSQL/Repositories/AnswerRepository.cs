using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.HeatMap;
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
        try
        {
            foreach (Answer ans in Answers)
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

    public async Task<List<AnswersReportQueryResponse>> GetAnswersByPollInstanceUuidCohortAsync(string PollUuid, string CohortId)
    {
        if (!int.TryParse(CohortId, out var cohortInt))
        {
            throw new ArgumentException("Invalid cohort Id format", nameof(CohortId));
        }
        IQueryable<AnswersReportQueryResponse> query =
            from A in _context.Answers
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            //Filter only answers related to the poll Uuid to look for
            where PI.Uuid == PollUuid
            join Stud in _context.Students on PI.StudentId equals Stud.Id
            join SC in _context.StudentCohorts on Stud.Id equals SC.StudentId
            //Filter only the answers related to the cohort to look for
            where cohortInt == 0 || SC.CohortId == cohortInt
            join PV in _context.PollVariables on A.PollVariableId equals PV.Id
            join Var in _context.Variables on PV.VariableId equals Var.Id
            join Component in _context.Components on Var.ComponentId equals Component.Id
            select new AnswersReportQueryResponse
            {
                ComponentName = Component.Name,
                PollInstanceId = PI.Id,
                Question = Var.Name,
                AnswerText = A.AnswerText,
                RiskLevel = A.RiskLevel,
                StudentEmail = Stud.Email
            };
        return await query.ToListAsync();
    }

    public async Task<List<AnswersReportQueryResponse>> GetAnswersByPollVariablesAsync(
    List<int> PollVariableIds)
    {
        IQueryable<AnswersReportQueryResponse> query =
            from A in _context.Answers
            join PV in _context.PollVariables on A.PollVariableId equals PV.Id
            where PollVariableIds.Count == 0 || PollVariableIds.Contains(PV.Id)
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            join Stud in _context.Students on PI.StudentId equals Stud.Id
            join Var in _context.Variables on PV.VariableId equals Var.Id
            join Component in _context.Components on Var.ComponentId equals Component.Id
            select new AnswersReportQueryResponse
            {
                ComponentName = Component.Name,
                PollInstanceId = PI.Id,
                Question = Var.Name,
                AnswerText = A.AnswerText,
                RiskLevel = A.RiskLevel,
                StudentEmail = Stud.Email
            };
        return await query.ToListAsync();
    }
}
