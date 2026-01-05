using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollVariableRepository(AppDbContext Context) : BaseRepository<Variable, PollVariableJoin>(Context, PollVariableMapper.ToDomain, PollVariableMapper.ToPersistenceVariable), IPollVariableRepository
    {
        public async Task<Variable?> GetByPollIdAndVariableIdAsync(int PollId, int VariableId)
        {
            var pollVariable = await _context.PollVariables
                .FirstOrDefaultAsync(PollVar => PollVar.PollId == PollId && PollVar.VariableId == VariableId);

            return pollVariable?.ToDomain();
        }

        public async Task<PagedResult<ErasCalculationsByPollDTO>?> GetByPollUuidVariableIdAsync(string PollUuid, List<int> VariableIds, Pagination Pagination)
        {
            var pollId = await _context.Polls
                .Where(P => P.Uuid == PollUuid)
                .Select(P => P.Id)
                .FirstOrDefaultAsync();

            var filteredVariableIds = await _context.PollVariables
                .Where(V => VariableIds.Contains(V.VariableId) && V.PollId == pollId)
                .Select(V => V.Id)
                .ToListAsync();

            var topRiskQuery = _context.ErasCalculationsByPoll
                .Where(Calculation => Calculation.PollUuid == PollUuid && filteredVariableIds.Contains(Calculation.PollVariableId))
                .Select(Result => new ErasCalculationsByPollDTO
                {
                    PollUuid = Result.PollUuid,
                    ComponentName = Result.ComponentName,
                    PollVariableId = Result.PollVariableId,
                    Question = Result.Question,
                    Position = Result.Position,
                    AnswerText = Result.AnswerText,
                    PollInstanceId = Result.PollInstanceId,
                    StudentName = Result.StudentName,
                    StudentEmail = Result.StudentEmail,
                    AnswerRisk = Result.AnswerRisk,
                    PollInstanceRiskSum = Result.PollInstanceRiskSum,
                    PollInstanceAnswersCount = Result.PollInstanceAnswersCount,
                    ComponentAverageRisk = Result.ComponentAverageRisk,
                    VariableAverageRisk = Result.VariableAverageRisk,
                    AnswerCount = Result.AnswerCount,
                    AnswerPercentage = Result.AnswerPercentage,
                    StudentId = Result.StudentId,
                    CohortId = Result.CohortId,
                });

            var Count = topRiskQuery.Distinct().Count();
            var Items = await topRiskQuery.Skip((Pagination.Page - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            return new PagedResult<ErasCalculationsByPollDTO>(Count, Items);
        }
        public async Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string PollUuid, string VariableIds)
        {
            var variableIdsArray = VariableIds.Split(',').Select(int.Parse).ToArray();

            var answers = await (from s in _context.Students
                                 join pi in _context.PollInstances on s.Id equals pi.StudentId
                                 join a in _context.Answers on pi.Id equals a.PollInstanceId
                                 join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                                 join v in _context.Variables on pv.VariableId equals v.Id
                                 join c in _context.Components on v.ComponentId equals c.Id
                                 where pi.Uuid == PollUuid && variableIdsArray.Contains(v.Id)
                                 select new { Answer = a.ToDomain(), Variable = v.ToDomain(), Student = s.ToDomain() }
                    ).ToListAsync();

            var groupedResults = answers
                .GroupBy(A => A.Student.Name)
                .Select(Group =>
                {
                    var averageRisk = Group.Average(G => G.Answer.RiskLevel);
                    var firstAnswer = Group.First();
                    return (
                        Answer: firstAnswer.Answer,
                        Variable: firstAnswer.Variable,
                        Student: firstAnswer.Student,
                        AverageRisk: averageRisk
                    );
                })
                .ToList();

            return groupedResults.Select(G => (G.Answer, G.Variable, G.Student)).ToList();
        }

        public async Task<List<Answer>> GetAnswersByPollUuidAsync(string PollUuid)
        {
            var answers = await (from pi in _context.PollInstances
                                 join a in _context.Answers on pi.Id equals a.PollInstanceId
                                 where pi.Uuid == PollUuid
                                 select new { Answer = a.ToDomain() }
                     ).ToListAsync();
            return [.. answers.Select(A => A.Answer)];
        }

    }
}
