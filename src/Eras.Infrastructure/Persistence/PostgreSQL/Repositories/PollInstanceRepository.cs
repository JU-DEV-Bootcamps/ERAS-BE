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
    private static bool IsValidAnswer(string? answerText) =>
        !string.IsNullOrEmpty(answerText) &&
        answerText != "-" &&
        !answerText.Equals("None", StringComparison.OrdinalIgnoreCase) &&
        !answerText.Equals("Ninguno", StringComparison.OrdinalIgnoreCase) &&
        !answerText.Equals("Ninguna", StringComparison.OrdinalIgnoreCase);

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

    public async Task<List<string>> GetImportedStudentsEmailsByPollName(string PollName)
    {
        return await _context.PollInstances
            .Where(p => _context.Polls.Any(poll => poll.Uuid == p.Uuid && poll.Name == PollName))
            .Include(p => p.Student)
            .Select(p => p.Student.Email)
            .Distinct()
            .ToListAsync();
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
            DateTime? EndDate,
            int? EvaluationId = null
    )
    {
        var query = _context.PollInstances
            .Include(PI => PI.Student)
            .Where(PI => PI.Uuid == PollUuid &&
                        _context.StudentCohorts.Any(SC => SC.StudentId == PI.StudentId && CohortId.Contains(SC.CohortId)));

        if (EvaluationId.HasValue)
        {
            query = query.Where(PI => PI.EvaluationId == EvaluationId.Value);
        }

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        if (StartDate.HasValue && EndDate.HasValue)
        {
            query = query.Where(PI => PI.FinishedAt >= StartDate.Value && PI.FinishedAt <= EndDate.Value);
        }
        else if (Days.HasValue && Days != 0 && LastVersion)
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
            query = query.Where(PI => PI.FinishedAt >= dateLimit && PI.LastVersion == pollVersion);
        }
        else
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(Days.HasValue ? -Days.Value : 0);
            query = query.Where(PI => PI.FinishedAt >= dateLimit && PI.LastVersion != pollVersion);
        }

        var totalCount = await query.CountAsync();

        var pollInstances = await query
            .OrderBy(PI => PI.FinishedAt)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var mapped = pollInstances
            .Select(PI => new PollInstance
            {
                Uuid = PI.Uuid,
                Student = PI.Student.ToDomain(),
                Audit = PI.Audit,
                LastVersion = PI.LastVersion,
                FinishedAt = PI.FinishedAt,
            })
            .ToList();

        return new PagedResult<PollInstance>(totalCount, mapped);
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
            .Distinct()
            .ToListAsync();

        int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();

        IQueryable<ErasCalculationsByPollDTO> reportQuery;

        if (LastVersion)
        {
            reportQuery =
            from A in _context.ErasCalculationsByPoll
            join PI in _context.PollInstances on A.PollInstanceId equals PI.Id
            where A.PollUuid == PollUuid
            where PI.FinishedAt >= startDate && PI.FinishedAt <= endDate
            where emailsInCohort.Contains(A.StudentEmail)
            where A.PollVersion == pollVersion
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                ComponentAverageRisk = A.ComponentAverageRisk,
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
            where A.PollVersion != pollVersion
            select new ErasCalculationsByPollDTO
            {
                ComponentName = A.ComponentName,
                ComponentAverageRisk = A.ComponentAverageRisk,
                Question = A.Question,
                Position = A.Position,
                AnswerText = A.AnswerText,
                VariableAverageRisk = A.VariableAverageRisk,
                AnswerPercentage = A.AnswerPercentage,
                StudentEmail = A.StudentEmail,
                AnswerRisk = A.AnswerRisk
            };
        }

        List<ErasCalculationsByPollDTO> rawResults = await reportQuery.ToListAsync();

        List<ErasCalculationsByPollDTO> results = [.. rawResults
            .GroupBy(A => new { A.ComponentName, A.Question, A.Position, A.AnswerText, A.StudentEmail })
            .Select(g => g.First())];

        var filteredResultsForMath = results.Where(A => IsValidAnswer(A.AnswerText)).ToList();

        var avgByComponent = filteredResultsForMath
            .GroupBy(A => A.ComponentName)
            .ToDictionary(
                g => g.Key,
                g => (decimal)Math.Round(g.Average(x => (double)x.AnswerRisk), 2)
            );

        List<AvgReportComponent> report = [.. results
            .GroupBy(A => A.ComponentName)
            .Select(AnsPerComp => new AvgReportComponent
            {
                Description = AnsPerComp.Key.ToUpper(),
                AverageRisk = avgByComponent.TryGetValue(AnsPerComp.Key, out var compAvg) ? compAvg : 0,
                Questions = [.. AnsPerComp
                    .OrderBy(A => A.VariableAverageRisk)
                    .GroupBy(A => new { A.Question, A.Position })
                    .Select(AnsPerVar =>
                    {
                        var validAnswers = AnsPerVar.Where(A => IsValidAnswer(A.AnswerText)).ToList();
                        var totalValid = validAnswers.Count;

                        return new AvgReportQuestions
                        {
                            Question = AnsPerVar.Key.Question,
                            Position = AnsPerVar.Key.Position,
                            AverageAnswer = validAnswers
                                .GroupBy(A => A.AnswerText)
                                .OrderByDescending(A => A.Count())
                                .FirstOrDefault()?.Key ?? "-",
                            AverageRisk = AnsPerVar.First().VariableAverageRisk,
                            AnswersDetails = [.. AnsPerVar
                                .GroupBy(A => A.AnswerText)
                                .Select(AnsGroup => new AnswerDetails
                                {
                                    AnswerText = AnsGroup.Key,
                                    AnswerPercentage = IsValidAnswer(AnsGroup.Key) && totalValid > 0
                                        ? Math.Round(AnsGroup.Count() * 100m / totalValid, 2)
                                        : 0,
                                    StudentsEmails = [.. AnsGroup.Select(A => A.StudentEmail)],
                                    RiskLevel = (int)AnsGroup.First().AnswerRisk
                                })]
                        };
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
            where PI.Audit.CreatedAt >= startDate && PI.FinishedAt <= endDate
            where PI.EvaluationId == EvaluationId
            select new ErasCalculationsByPollDTO
            {
                ComponentId = A.ComponentId,
                ComponentName = A.ComponentName,
                ComponentAverageRisk = A.ComponentAverageRisk,
                VariableAverageRisk = A.VariableAverageRisk,
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

        var avgByComponent = results
    .GroupBy(A => A.ComponentName)
    .ToDictionary(
        g => g.Key,
        g => (decimal)Math.Round(g.Average(x => (double)x.AnswerRisk), 2)
    );

        var avgByQuestion = results
            .GroupBy(A => new { A.ComponentName, A.Position, A.Question })
            .ToDictionary(
                g => g.Key,
                g => g.First().VariableAverageRisk
            );
            
        List<CountReportComponent> report = [.. results
            .OrderBy(A => A.ComponentId)
            .GroupBy(A => A.ComponentName)
            .Select(AnsPerComp => new CountReportComponent {
                Description = AnsPerComp.Key.ToUpper(),
                AverageRisk = avgByComponent.TryGetValue(AnsPerComp.Key, out var compAvg) ? (double)compAvg : 0,
                Questions = [.. AnsPerComp
                    .OrderBy(Q => Q.AnswerRisk)
                    .GroupBy(Q => new { Q.Position, Q.Question })
                    .Select(AnsPerQuestion => new CountReportQuestion {
                        AverageRisk = avgByQuestion.TryGetValue(
                            new { ComponentName = AnsPerComp.Key, AnsPerQuestion.Key.Position, AnsPerQuestion.Key.Question },
                            out var qAvg) ? (double)qAvg : 0,
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
    public string ComputeAnswersHash(PollDTO incomingPoll) => ComputeHashFromDTO(incomingPoll);

    public async Task<PollInstance?> FindMatchingSourceInstanceAsync(int studentId, int currentPollInstanceId, PollDTO incomingPoll)
    {
        var incomingHash = ComputeHashFromDTO(incomingPoll);

        // Fast path: indexed lookup on the persisted hash (ix_poll_instances_student_id_answers_hash).
        var match = await _context.PollInstances
            .Where(pi => pi.StudentId == studentId
                      && pi.SourcePollInstanceId == null
                      && pi.Id != currentPollInstanceId
                      && pi.AnswersHash == incomingHash)
            .OrderByDescending(pi => pi.FinishedAt)
            .FirstOrDefaultAsync();
        if (match != null) return PollInstanceMapper.ToDomain(match);

        // Fallback for legacy rows persisted before AnswersHash existed: hash their answers in memory.
        var legacyCandidates = await _context.PollInstances
            .Where(pi => pi.StudentId == studentId
                      && pi.SourcePollInstanceId == null
                      && pi.Id != currentPollInstanceId
                      && pi.AnswersHash == null)
            .Include(pi => pi.Answers)
            .OrderByDescending(pi => pi.FinishedAt)
            .ToListAsync();

        var foundHash = legacyCandidates.FirstOrDefault(pi =>
            ComputeHashFromAnswers(pi.Answers) == incomingHash);
        return foundHash != null ? PollInstanceMapper.ToDomain(foundHash) : null;
    }

    private string ComputeHashFromDTO(PollDTO pollDTO)
    {
        var entries = pollDTO.Components
            .SelectMany(c => c.Variables)
            .Select(v => $"{v.Answer.Answer}")
            .OrderBy(x => x);

        return Hash(string.Join("|", entries));
    }

    private string ComputeHashFromAnswers(ICollection<AnswerEntity> answers)
    {
        var entries = answers
            .Select(a => $"{a.AnswerText}")
            .OrderBy(x => x);

        return Hash(string.Join("|", entries));
    }

    private string Hash(string content) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
}
