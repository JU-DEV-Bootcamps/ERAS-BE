using System.Security.Cryptography;
using System.Text;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs.Views;
using Eras.Application.Models.Consolidator;
using Eras.Application.Utils;
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
        PollInstanceEntity? results = await _context.PollInstances.FirstOrDefaultAsync(Poll => Poll.Uuid.Equals(Uuid) && Poll.StudentId == StudentId);
        return results?.ToDomain();
    }

    public async Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId, int EvaluationId)
    {
        PollInstanceEntity? results = await _context.PollInstances.FirstOrDefaultAsync(Poll => Poll.Uuid.Equals(Uuid) && Poll.StudentId == StudentId && Poll.EvaluationId == EvaluationId);
        return results?.ToDomain();
    }

    public async Task<bool> ExistsByPollNameAndStudentEmailAsync(string PollName, string StudentEmail)
    {
        return await _context.PollInstances
            .Include(p => p.Student)
            .AnyAsync(p => _context.Polls
                .Any(poll => poll.Uuid == p.Uuid && poll.Name == PollName) &&
                p.Student.Email == StudentEmail);
    }

    public async Task<IEnumerable<PollInstance>> GetByLastDays(int Days, bool LastVersion, string PollUuid)
    {
        DateTime dateLimit = DateTime.UtcNow.AddDays(-Days);

        int? pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => (int?)A.LastVersion)
            .FirstOrDefault() ?? throw new InvalidOperationException($"No se encontró una encuesta con UUID {PollUuid}");
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

    public async Task<PagedResult<PollInstance>> GetByCohortIdAndLastDays(
            int Page,
            int PageSize,
            int[] CohortId,
            int? Days,
            bool LastVersion,
            string PollUuid,
            DateTime? StartDate,
            DateTime? EndDate 
    )
    {
        var query = _context.PollInstances.Include(PI => PI.Student)
            .Join(_context.StudentCohorts,
                PollInstance => PollInstance.StudentId,
                StudentCohort => StudentCohort.StudentId,
                (PollInstance, StudentCohort) => new { pollInstance = PollInstance, studentCohort = StudentCohort })
            .Where(Joined => CohortId.Contains(Joined.studentCohort.CohortId) && Joined.pollInstance.Uuid == PollUuid);

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        if (StartDate.HasValue && EndDate.HasValue)
        {
            query = query.Where(PI =>
                PI.pollInstance.FinishedAt >= StartDate.Value &&
                PI.pollInstance.FinishedAt <= EndDate.Value);
        }
        else if (Days.HasValue && Days != 0 && LastVersion)
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
            query = query.Where(PI =>
                PI.pollInstance.FinishedAt >= dateLimit &&
                PI.pollInstance.LastVersion == pollVersion);
        }
        else
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(Days.HasValue ? -Days.Value : 0);
            query = query.Where(PI =>
                PI.pollInstance.FinishedAt >= dateLimit &&
                PI.pollInstance.LastVersion != pollVersion);
        }

        var totalCount = await query.Distinct().CountAsync();
        var pollInstances = await
            query
            .Distinct()
            .Select(Resp => new PollInstance
            {
                Uuid = Resp.pollInstance.Uuid,
                Student = Resp.pollInstance.Student.ToDomain(),
                Audit = Resp.pollInstance.Audit,
                LastVersion = Resp.pollInstance.LastVersion,
                FinishedAt = Resp.pollInstance.FinishedAt,
            })
            .OrderBy(P => P.FinishedAt)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return new PagedResult<PollInstance>(totalCount, pollInstances);
    }

    public async Task<AvgReportResponseVm> GetReportByPollCohortAsync(
    string PollUuid, List<int> CohortIds, bool LastVersion,
    DateTime startDate, DateTime endDate)
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

        if (LastVersion)
        {
            reportQuery =
            from A in _context.ErasCalculationsByPoll
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            where A.PollUuid == PollUuid
            where PI.FinishedAt >= startDate && PI.FinishedAt <= endDate
            where emailsInCohort.Contains(A.StudentEmail)
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                Question = A.Question,
                Position = A.Position,
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
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id 
            where A.PollUuid == PollUuid
            where PI.FinishedAt >= startDate && PI.FinishedAt <= endDate 
            where emailsInCohort.Contains(A.StudentEmail)
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                Question = A.Question,
                Position = A.Position,
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
                .GroupBy(A => new { A.Question, A.Position })
                .Select(AnsPerVar => new AvgReportQuestions
                {
                    Question = AnsPerVar.Key.Question,
                    Position = AnsPerVar.Key.Position,
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

    public async Task<CountReportResponseVm> GetCountReportByVariablesAsync(string PollUuid, List<int> CohortIds, List<int> VariableIds, bool LastVersion, DateTime startDate, DateTime endDate, int? EvaluationId)
    {

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();
        IQueryable<ErasCalculationsByPollDTO> reportQuery =
            from A in _context.ErasCalculationsByPoll
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            where A.PollUuid == PollUuid
            where CohortIds.Contains(A.CohortId)
            where VariableIds.Contains(A.PollVariableId)
            //where PI.FinishedAt >= startDate && PI.FinishedAt <= endDate -- OLD conditional
            where PI.Audit.CreatedAt >= startDate && PI.FinishedAt <= endDate
            where PI.EvaluationId == EvaluationId
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                ComponentAverageRisk = A.ComponentAverageRisk,
                AnswerText = A.AnswerText,
                AnswerRisk = A.AnswerRisk,
                Question = A.Question,
                Position = A.Position,
                StudentName = A.StudentName,
                StudentEmail = A.StudentEmail,
                CohortId = A.CohortId,
                CohortName = A.CohortName,
                PollVersion = A.PollVersion
            };
        
        List<ErasCalculationsByPollDTO> results = await reportQuery.ToListAsync();

        List<CountReportComponent> report = [.. results
        .GroupBy(A => A.ComponentName)
        .Select(AnsPerComp => new CountReportComponent {
            Description = AnsPerComp.Key.ToUpper(),
            AverageRisk = (double)Math.Round(AnsPerComp.Average(A => A.AnswerRisk), 2),
            Questions = [.. AnsPerComp
                .OrderBy(Q => Q.AnswerRisk)
                .GroupBy(Q => new { Q.Position, Q.Question })
                .Select(AnsPerQuestion => new CountReportQuestion {
                    AverageRisk = (double)Math.Round(AnsPerQuestion.Average(A => A.AnswerRisk),2),
                    Position = AnsPerQuestion.Key.Position,
                    Question = AnsPerQuestion.Key.Question,
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

    public new async Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.PollInstances
            .Where(pi => pi.FinishedAt >= DateTime.SpecifyKind(startDate, DateTimeKind.Utc)
                    && pi.FinishedAt <= DateTime.SpecifyKind(endDate, DateTimeKind.Utc))
            .CountAsync();
    }

    public async Task<bool> ExistsForStudentAndEvaluationAsync(int StudentId, string PollUuid, int EvaluationId)
    {
        return await _context.PollInstances
            .AsNoTracking()
            .AnyAsync(p => p.StudentId == StudentId
                        && p.Uuid == PollUuid
                        && p.EvaluationId == EvaluationId);
    }

    public async Task SetSourceInstanceAsync(int pollInstanceId, int sourceInstanceId)
    {
        var pi = await _context.PollInstances.FindAsync(pollInstanceId);
        if (pi != null)
        {
            pi.SourcePollInstanceId = sourceInstanceId;
            await _context.SaveChangesAsync();
        }
    }

    private string ComputeAnswerHash(PollDTO pollDTO)
    {
        var answers = pollDTO.Components
            .SelectMany(c => c.Variables)
            .Select(v => $"{v.Answer.PollVariableId}:{v.Answer}:{v.Answer.Score}");

        var content = string.Join("|", answers.OrderBy(x => x));

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }
    public async Task<PollInstance?> FindMatchingSourceInstanceAsync(int studentId, int currentPollInstanceId, PollDTO incomingPoll)
    {
        var incomingHash = ComputeHashFromDTO(incomingPoll);

        var candidates = await _context.PollInstances
            .Where(pi => pi.StudentId == studentId
                      && pi.SourcePollInstanceId == null
                      && pi.Id != currentPollInstanceId)
            .Include(pi => pi.Answers)
            .OrderByDescending(pi => pi.FinishedAt)
            .ToListAsync();

        return PollInstanceMapper.ToDomain(candidates.FirstOrDefault(pi =>
            ComputeHashFromAnswers(pi.Answers) == incomingHash));
    }

    // Hash desde el DTO (entrante)
    private string ComputeHashFromDTO(PollDTO pollDTO)
    {
        var entries = pollDTO.Components
            .SelectMany(c => c.Variables)
            .Select(v => $"{v.Answer.PollVariableId}:{v.Answer}:{v.Answer.Score}")
            .OrderBy(x => x);

        return Hash(string.Join("|", entries));
    }

    // Hash desde las answers ya guardadas en DB
    private string ComputeHashFromAnswers(ICollection<AnswerEntity> answers)
    {
        var entries = answers
            .Select(a => $"{a.PollVariableId}:{a.AnswerText}:{a.RiskLevel}")
            .OrderBy(x => x);

        return Hash(string.Join("|", entries));
    }

    private string Hash(string content) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
}
