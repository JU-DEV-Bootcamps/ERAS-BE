using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
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

    public async Task<AvgReportResponseVm> GetAnswersByPollInstanceUuidAsync(
    string PollUuid, string CohortId)
    {
        if (!int.TryParse(CohortId, out var cohortInt))
        {
            throw new ArgumentException("Invalid cohort Id format", nameof(CohortId));
        }

        var query =
            from A in _context.Answers
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            //Filter only answers related to the poll Uuid to look for
            where PI.Uuid == PollUuid
            join Stud in _context.Students on PI.StudentId equals Stud.Id
            join SC in _context.StudentCohorts on Stud.Id equals SC.StudentId
            //Filter only the answers related to the cohort to look for
            where SC.CohortId == cohortInt
            join PV in _context.PollVariables on A.PollVariableId equals PV.Id
            join Var in _context.Variables on PV.VariableId equals Var.Id
            join Component in _context.Components on Var.ComponentId equals Component.Id
            select new
            {
                Component = Component.Name,
                PollInstanceId = PI.Id,
                Question = Var.Name,
                A.AnswerText,
                A.RiskLevel,
                StudentEmail = Stud.Email
            };
        var results = await query.ToListAsync();

        List<AvgReportComponent> report = await query
        .GroupBy(A => A.Component)
        .Select(AnsPerComp => new AvgReportComponent
        {
            Description = AnsPerComp.Key,
            AverageRisk = AnsPerComp.Average(Ans => Ans.RiskLevel),
            Questions = AnsPerComp
                .OrderByDescending(Ans => Ans.RiskLevel)
                .GroupBy(A => A.Question)
                .Select(AnsPerVar => new AvgReportQuestions
                {
                    Question = AnsPerVar.Key,
                    AverageRisk = AnsPerVar.Average(Ans => Ans.RiskLevel),
                    Answer = AnsPerVar.First(A => A.RiskLevel < AnsPerComp.Average(Ans => Ans.RiskLevel)).AnswerText,
                }).ToList()
        }).ToListAsync();
        var pollCount = await _context.PollInstances.CountAsync(PollInstance => PollInstance.Uuid == PollUuid);
        return new AvgReportResponseVm { Components = report, PollCount = pollCount };
    }
}
