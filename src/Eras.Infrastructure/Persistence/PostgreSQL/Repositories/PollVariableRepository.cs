using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class PollVariableRepository : BaseRepository<Variable, PollVariableJoin>, IPollVariableRepository
    {
        public PollVariableRepository(AppDbContext Context)
            : base(Context, PollVariableMapper.ToDomain, PollVariableMapper.ToPersistenceVariable)
        {

        }
        public async Task<Variable?> GetByPollIdAndVariableIdAsync(int PollId, int VariableId)
        {
            var pollVariable = await _context.PollVariables
                .FirstOrDefaultAsync(PollVar => PollVar.PollId == PollId && PollVar.VariableId == VariableId);

            return pollVariable?.ToDomain();
        }

        public async Task<List<ErasCalculationsByPollDTO>?> GetByPollUuidVariableIdAsync(string PollUuid, int VaribaleId)
        {
            var topRisk = await _context.ErasCalculationsByPoll
                .Where(Calculation => Calculation.PollUuid == PollUuid && Calculation.PollVariableId == VaribaleId)
                .Select(Result => new
                        ErasCalculationsByPollDTO
                {
                    PollUuid = Result.PollUuid,
                    ComponentName = Result.ComponentName,
                    PollVariableId = Result.PollVariableId,
                    Question = Result.Question,
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
                    AnswerPercentage = Result.AnswerPercentage
                })
                .ToListAsync();

            return topRisk;

            /* foreach (var result in topRisk)
            {
                Console.WriteLine($"PollUuid: {result.PollUuid}, PollVariableId: {result.PollVariableId}, RiskCount: {result.RiskCount}");
            }

            Console.WriteLine($"==========================================================");
            Console.WriteLine($"PollUuid: {PollUuid}, PollVariableId: {VaribaleId}");
            Console.WriteLine($"==========================================================");

            var answers = await (from s in _context.Students
                                 join pi in _context.PollInstances on s.Id equals pi.StudentId
                                 join a in _context.Answers on pi.Id equals a.PollInstanceId
                                 join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                                 join v in _context.Variables on pv.VariableId equals v.Id
                                 join c in _context.Components on v.ComponentId equals c.Id
                                 where pi.Uuid == PollUuid && v.Id == VaribaleId
                                 select new { Answer = a.ToDomain(), Variable = v.ToDomain(), Student = s.ToDomain() }
                     ).ToListAsync();

            return [.. answers.Select(A => (A.Answer, A.Variable, A.Student))]; */
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

        public Task<List<(Answer Answer, Variable Variable, Student Student)>> GetByPollUuidAsync(string PollUuid, int VaribaleId) => throw new NotImplementedException();
    }
}
