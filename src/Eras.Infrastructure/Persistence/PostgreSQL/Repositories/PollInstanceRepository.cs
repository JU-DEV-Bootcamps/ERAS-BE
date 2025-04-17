using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PollInstanceRepository(AppDbContext Context) : BaseRepository<PollInstance, PollInstanceEntity>(Context, PollInstanceMapper.ToDomain, PollInstanceMapper.ToPersistence), IPollInstanceRepository
{
    public async Task<PollInstance?> GetByUuidAsync(string Uuid)
    {
        PollInstanceEntity? pollInstance = await _context.PollInstances
            .FirstOrDefaultAsync(PollInstance => PollInstance.Uuid == Uuid);

        return pollInstance?.ToDomain();
    }

    public async Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId)
    {
        PollInstanceEntity? results = await _context.PollInstances.FirstOrDefaultAsync(Poll => Poll.Uuid.Equals(Uuid) && Poll.StudentId.Equals(StudentId));
        return results?.ToDomain();
    }


    public async Task<IEnumerable<PollInstance>> GetByLastDays(int Days)
    {
        DateTime dateLimit = DateTime.UtcNow.AddDays(-Days);
        List<PollInstanceEntity> pollInstanceCounts = await _context.PollInstances
        .Include(PI => PI.Student)
        .Where(PI => PI.FinishedAt >= dateLimit)
        .ToListAsync();

        return pollInstanceCounts.Select(PollInstanceMapper.ToDomain);
    }

    public async Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days)
    {
        IQueryable<PollInstanceEntity> query = _context.PollInstances.Include(PI => PI.Student);

        if (CohortId.HasValue && CohortId != 0)
        {
            query = query
                .Join(_context.StudentCohorts,
                    PollInstance => PollInstance.StudentId,
                    StudentCohort => StudentCohort.StudentId,
                    (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort = StudentCohort })
                .Where(Joined => Joined.studentCohort.CohortId == CohortId.Value)
                .Select(Joined => Joined.pollInstance);
        }

        if (Days.HasValue && Days != 0)
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
            query = query.Where(PI => PI.FinishedAt >= dateLimit);
        }

        List<PollInstanceEntity> pollInstances = await query.Distinct().ToListAsync();
        return [.. pollInstances.Select(PollInstanceMapper.ToDomain)];
    }

    public async Task<IEnumerable<Answer>> GetAnswersByPollInstanceUuidAsync(string PollUuid, int CohortId)
    {
        List<AnswerEntity> answers = await _context.Answers
            .Include(Answer => Answer.PollInstance)
                .ThenInclude(PollInstance => PollInstance.Student)
                    .ThenInclude(Student => Student.StudentCohorts)
            .Include(Answer => Answer.PollVariable)
                .ThenInclude(PollV => PollV.Variable)
                    .ThenInclude(Var => Var.Component)
            .Where(Answer => Answer.PollInstance.Uuid == PollUuid &&
                            (CohortId == 0 || Answer.PollInstance.Student.StudentCohorts.Any(Cohort => Cohort.CohortId == CohortId)))
            .ToListAsync();

        return [.. answers.Select(AnswerMapper.ToDomainWithRelations)];
    }

    public async Task<AvgReportResponseVm> GetAvgReportByCohortAsync(
    string CohortId,
    string PollUuid)
    {
        if (!int.TryParse(CohortId, out var cohortInt))
        {
            throw new ArgumentException("Invalid cohort Id format", nameof(CohortId));
        }

        IQueryable<AvgReportResponseVm> query =
            from A in _context.Answers
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            //Filter only answers related to the poll Uuid to look for
            where PI.Uuid == PollUuid.ToString()
            join Stud in _context.Students on PI.StudentId equals Stud.Id
            join SC in _context.StudentCohorts on Stud.Id equals SC.StudentId
            join Cohort in _context.Cohorts on SC.CohortId equals Cohort.Id
            //Filter only the answers related to the cohort to look for
            where Cohort.Id == cohortInt
            join PV in _context.PollVariables on A.PollInstanceId equals PV.PollId
            join Var in _context.Variables on PV.VariableId equals Var.Id
            join Comp in _context.Components on Var.ComponentId equals Comp.Id
            group A by new { Comp.Id, Comp.Name } into answersByComponents
            select (new AvgReportResponseVm
            {
                Components = answersByComponents.Select(Comp => new AvgReportComponent
                {
                    Description = answersByComponents.Key.Name,
                    AverageRisk = answersByComponents.Average(Ans => Ans.RiskLevel),
                    Questions = answersByComponents.Select(Ans => new AvgReportQuestions
                    {
                        Question = Ans.PollVariable.Variable.Name,
                        Answer = Ans.AnswerText,
                        AverageRisk = answersByComponents.Average(Ans => Ans.RiskLevel)
                    }).ToList()
                })
            });
        return await query.FirstOrDefaultAsync() ?? new AvgReportResponseVm { Components = [] };
    }
}
