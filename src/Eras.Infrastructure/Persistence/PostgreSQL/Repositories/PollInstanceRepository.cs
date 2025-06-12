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


    public async Task<IEnumerable<PollInstance>> GetByLastDays(int Days, bool LastVersion, string PollUuid)
    {
        DateTime dateLimit = DateTime.UtcNow.AddDays(-Days);

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();
        
        List<PollInstanceEntity> pollInstanceCounts;
        
        if (LastVersion)
        {
            pollInstanceCounts = await _context.PollInstances
                .Include(PI => PI.Student)
                .Where(PI => PI.FinishedAt >= dateLimit && PI.LastVersion == pollVersion)
                .ToListAsync();
        }
        else
        {
            pollInstanceCounts = await _context.PollInstances
                .Include(PI => PI.Student)
                .Where(PI => PI.FinishedAt >= dateLimit && PI.LastVersion != pollVersion)
                .ToListAsync();
        }

        return pollInstanceCounts.Select(PollInstanceMapper.ToDomain);
    }

    public async Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days, bool LastVersion, string PollUuid)
    {
        IQueryable<PollInstanceEntity> query = _context.PollInstances.Include(PI => PI.Student);

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        if (CohortId.HasValue && CohortId != 0 && LastVersion)
        {
            query = query
                .Join(_context.StudentCohorts,
                    PollInstance => PollInstance.StudentId,
                    StudentCohort => StudentCohort.StudentId,
                    (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort = StudentCohort })
                .Where(Joined => Joined.studentCohort.CohortId == CohortId.Value && Joined.pollInstance.LastVersion == pollVersion)
                .Select(Joined => Joined.pollInstance);
        } else
        {
            query = query
                .Join(_context.StudentCohorts,
                    PollInstance => PollInstance.StudentId,
                    StudentCohort => StudentCohort.StudentId,
                    (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort = StudentCohort })
                .Where(Joined => Joined.studentCohort.CohortId == CohortId.Value && Joined.pollInstance.LastVersion != pollVersion)
                .Select(Joined => Joined.pollInstance);
        }

        if (Days.HasValue && Days != 0 && LastVersion)
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
            query = query.Where(PI => PI.FinishedAt >= dateLimit && PI.LastVersion == pollVersion);
        } 
        else
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
            query = query.Where(PI => PI.FinishedAt >= dateLimit && PI.LastVersion == pollVersion);
        }

        List<PollInstanceEntity> pollInstances = await query.Distinct().ToListAsync();
        return [.. pollInstances.Select(PollInstanceMapper.ToDomain)];
    }

    public async Task<AvgReportResponseVm> GetReportByPollCohortAsync(
    string PollUuid, List<int> CohortIds, bool LastVersion)
    {
        List<string> emailsInCohort = await _context.StudentCohorts
            .Where(SC => CohortIds.Contains(SC.CohortId))
            .Join(_context.Students,
                SC => SC.StudentId,
                S => S.Id,
                (SC, S) => new { SC, S })
            .Select(SC => SC.S.Email)
            .ToListAsync();
        IQueryable<ErasCalculationsByPollDTO> reportQuery;

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        if (LastVersion) {
            reportQuery =
            from A in _context.ErasCalculationsByPoll
            where A.PollUuid == PollUuid
            where A.PollVersion == pollVersion
            where emailsInCohort.Contains(A.StudentEmail)
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                Question = A.Question,
                AnswerText = A.AnswerText,
                VariableAverageRisk = A.VariableAverageRisk,
                AnswerPercentage = A.AnswerPercentage,
                StudentEmail = A.StudentEmail,
                AnswerRisk = A.AnswerRisk
            };
        }
        else
        {
            reportQuery =
            from A in _context.ErasCalculationsByPoll
            where A.PollUuid == PollUuid
            where A.PollVersion != pollVersion
            where emailsInCohort.Contains(A.StudentEmail)
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                Question = A.Question,
                AnswerText = A.AnswerText,
                VariableAverageRisk = A.VariableAverageRisk,
                AnswerPercentage = A.AnswerPercentage,
                StudentEmail = A.StudentEmail,
                AnswerRisk = A.AnswerRisk
            };
        }   

        List<ErasCalculationsByPollDTO> results = await reportQuery.ToListAsync();

        List<AvgReportComponent> report = [.. results
        .GroupBy(A => A.ComponentName)
        .Select(AnsPerComp => new AvgReportComponent
        {
            Description = AnsPerComp.Key.ToUpper(),
            AverageRisk = Math.Round(AnsPerComp.Average(X => X.AnswerRisk), 2),
            Questions = [.. AnsPerComp
                .OrderBy(A => A.VariableAverageRisk)
                .GroupBy(A => A.Question)
                .Select(AnsPerVar => new AvgReportQuestions
                {
                    Question = AnsPerVar.Key,
                    AverageAnswer = AnsPerVar.GroupBy(A => A.AnswerText).OrderByDescending(A => A.Count()).First().Key,
                    AverageRisk = Math.Round(AnsPerVar.Average(X => X.AnswerRisk), 2),
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

    public new async Task<PollInstance> UpdateAsync(PollInstance Entity)
    {

        var existingEntity = await _context.Set<PollInstanceEntity>().FindAsync(Entity.Id);

        if (existingEntity != null)
        {
            var updatedEntity = PollInstanceMapper.ToPersistence(Entity);
            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync();
        }

        return Entity;
    }

    public async Task<CountReportResponseVm> GetCountReportByVariablesAsync(string PollUuid, List<int> CohortIds, List<int> VariableIds, bool LastVersion)
    {

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();
        IQueryable<ErasCalculationsByPollDTO> reportQuery;
        if (LastVersion)
        {
            reportQuery =
            from A in _context.ErasCalculationsByPoll
            where A.PollUuid == PollUuid
            where A.PollVersion == pollVersion
            where CohortIds.Contains(A.CohortId)
            where VariableIds.Contains(A.PollVariableId)
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                ComponentAverageRisk = A.ComponentAverageRisk,
                AnswerText = A.AnswerText,
                AnswerRisk = A.AnswerRisk,
                Question = A.Question,
                StudentName = A.StudentName,
                StudentEmail = A.StudentEmail,
                CohortId = A.CohortId,
                CohortName = A.CohortName,
                PollVersion = A.PollVersion
            };
        }
        else
        {
            reportQuery =
            from A in _context.ErasCalculationsByPoll
            where A.PollUuid == PollUuid
            where A.PollVersion != pollVersion
            where CohortIds.Contains(A.CohortId)
            where VariableIds.Contains(A.PollVariableId)
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                ComponentAverageRisk = A.ComponentAverageRisk,
                AnswerText = A.AnswerText,
                AnswerRisk = A.AnswerRisk,
                Question = A.Question,
                StudentName = A.StudentName,
                StudentEmail = A.StudentEmail,
                CohortId = A.CohortId,
                CohortName = A.CohortName,
                PollVersion = A.PollVersion
            };
        }

        List<ErasCalculationsByPollDTO> results = await reportQuery.ToListAsync();

        List<CountReportComponent> report = [.. results
        .GroupBy(A => A.ComponentName)
        .Select(AnsPerComp => new CountReportComponent {
            Description = AnsPerComp.Key.ToUpper(),
            AverageRisk = (double)Math.Round(AnsPerComp.Average(A=>A.AnswerRisk), 2),
            Questions = [.. AnsPerComp
                .OrderBy(Q => Q.AnswerRisk)
                .GroupBy(Q => Q.Question)
                .Select(AnsPerQuestion => new CountReportQuestion {
                    AverageRisk = (double)Math.Round(AnsPerQuestion.Average(A => A.AnswerRisk),2),
                    Question = AnsPerQuestion.Key,
                    Answers = [.. AnsPerQuestion
                        .GroupBy(A => A.AnswerRisk)
                        .Select(AnsPerAns => new CountReportAnswer
                        {
                            AnswerRisk = AnsPerAns.Key,
                            Count = AnsPerAns.Count(),
                            Students = [.. AnsPerAns.Select(S => new CountReportStudent {
                                    AnswerText = S.AnswerText,
                                    Name = S.StudentEmail,
                                    Email = S.StudentEmail,
                                    CohortId = S.CohortId
                            })]
                    })]
                })]
        })];
        return new CountReportResponseVm { Components = report };
    }
}
