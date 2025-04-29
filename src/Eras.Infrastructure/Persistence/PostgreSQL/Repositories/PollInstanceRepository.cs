using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Views;
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

        if (CohortId.HasValue)
        {
            query = query
                .Join(_context.StudentCohorts,
                    PollInstance => PollInstance.StudentId,
                    StudentCohort => StudentCohort.StudentId,
                    (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort = StudentCohort })
                //If CohortId is 0, it means all cohorts are selected
                .Where(Joined => CohortId == 0 || Joined.studentCohort.CohortId == CohortId.Value)
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

    public async Task<AvgReportResponseVm> GetReportByPollCohortAsync(
    string PollUuid, int CohortId)
    {
        List<string> emailsInCohort = await _context.StudentCohorts
            .Where(SC => CohortId == 0 || SC.CohortId == CohortId)
            .Join(_context.Students,
                SC => SC.StudentId,
                S => S.Id,
                (SC, S) => new { SC, S })
            .Select(SC => SC.S.Email)
            .ToListAsync();

        IQueryable<ErasCalculationsByPollDTO> reportQuery =
        from A in _context.ErasCalculationsByPoll
        where A.PollUuid == PollUuid
        where emailsInCohort.Contains(A.StudentEmail)
        orderby A.AnswerRisk descending
        select new ErasCalculationsByPollDTO
        {
            PollUuid = A.PollUuid,
            ComponentName = A.ComponentName,
            PollVariableId = A.PollVariableId,
            Question = A.Question,
            AnswerText = A.AnswerText,
            PollInstanceId = A.PollInstanceId,
            StudentName = A.StudentName,
            StudentEmail = A.StudentEmail,
            AnswerRisk = A.AnswerRisk,
            PollInstanceRiskSum = A.PollInstanceRiskSum,
            PollInstanceAnswersCount = A.PollInstanceAnswersCount,
            ComponentAverageRisk = A.ComponentAverageRisk,
            VariableAverageRisk = A.VariableAverageRisk,
            AnswerCount = A.AnswerCount,
            AnswerPercentage = A.AnswerPercentage
        };

        List<ErasCalculationsByPollDTO> results = await reportQuery.ToListAsync();

        List<AvgReportComponent> report = [.. results
        .GroupBy(A => A.ComponentName)
        .Select(AnsPerComp => new AvgReportComponent
        {
            Description = AnsPerComp.Key.ToUpper(),
            AverageRisk = Math.Round(AnsPerComp.First().ComponentAverageRisk, 2),
            Questions = [.. AnsPerComp
                .OrderByDescending(A => A.VariableAverageRisk)
                .GroupBy(A => A.Question)
                .Select(AnsPerVar => new AvgReportQuestions
                {
                    Question = AnsPerVar.Key,
                    AverageAnswer = AnsPerVar.GroupBy(A => A.AnswerText).OrderByDescending(A => A.Count()).First().Key,
                    AverageRisk = Math.Round(AnsPerVar.First().VariableAverageRisk, 2),
                    AnswersDetails = [.. AnsPerVar
                        .GroupBy(A => A.AnswerText)
                        .Select(AnsPerVar => new AnswerDetails
                        {
                            AnswerText = AnsPerVar.Key,
                            AnswerPercentage = AnsPerVar.First().AnswerPercentage,
                            StudentsEmails = [.. AnsPerVar.Select(A => A.StudentEmail)]
                        })]
                })]
        })];
        return new AvgReportResponseVm { Components = report, PollCount = results.DistinctBy(R => R.StudentEmail).Count() };
    }
}
