using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

/// <summary>One answer of one student, resolved to the evaluation it was imported into.</summary>
public sealed class EvaluationAnswerRow
{
    public int PollInstanceId { get; init; }
    public int StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public string StudentEmail { get; init; } = string.Empty;
    public string PollUuid { get; init; } = string.Empty;
    public int ComponentId { get; init; }
    public string ComponentName { get; init; } = string.Empty;
    public int VariableId { get; init; }
    public string VariableName { get; init; } = string.Empty;
    public int Position { get; init; }
    public int PollVariableId { get; init; }
    public string AnswerText { get; init; } = string.Empty;
    public decimal AnswerRisk { get; init; }
    public int AnswerVersion { get; init; }
    public int PollLastVersion { get; init; }
}

/// <summary>
/// Evaluation-scoped read model used by the reports. It starts from the poll instances of one
/// evaluation (small, indexed set) and only then joins answers and structure, instead of reading
/// the whole-database <c>vErasCalculationByPoll</c> view. An evaluation can hold several polls, so
/// nothing here filters by a single poll uuid.
/// </summary>
public static class EvaluationAnswerQuery
{
    public static IQueryable<EvaluationAnswerRow> Build(
        AppDbContext Context, int EvaluationId, DateTime? StartDate = null, DateTime? EndDate = null)
    {
        var instances = Context.PollInstances.AsNoTracking()
            .Where(PI => PI.EvaluationId == EvaluationId);

        if (StartDate.HasValue && EndDate.HasValue)
        {
            instances = instances.Where(PI => PI.FinishedAt >= StartDate.Value && PI.FinishedAt <= EndDate.Value);
        }

        // Re-imported instances point to the instance that holds the answers (SourcePollInstanceId).
        return
            from PI in instances
            join A in Context.Answers on (PI.SourcePollInstanceId ?? PI.Id) equals A.PollInstanceId
            join PV in Context.PollVariables on A.PollVariableId equals PV.Id
            join P in Context.Polls on PV.PollId equals P.Id
            join V in Context.Variables on PV.VariableId equals V.Id
            join C in Context.Components on V.ComponentId equals C.Id
            join S in Context.Students on PI.StudentId equals S.Id
            select new EvaluationAnswerRow
            {
                PollInstanceId = PI.Id,
                StudentId = S.Id,
                StudentName = S.Name,
                StudentEmail = S.Email,
                PollUuid = P.Uuid,
                ComponentId = C.Id,
                ComponentName = C.Name,
                VariableId = V.Id,
                VariableName = V.Name,
                Position = V.Position,
                PollVariableId = PV.Id,
                AnswerText = A.AnswerText,
                AnswerRisk = A.RiskLevel,
                AnswerVersion = A.Version.VersionNumber,
                PollLastVersion = P.LastVersion
            };
    }

    /// <summary>Keeps students that belong to at least one of the cohorts (EXISTS, no duplicated rows).</summary>
    public static IQueryable<EvaluationAnswerRow> InCohorts(
        this IQueryable<EvaluationAnswerRow> Query, AppDbContext Context, List<int> CohortIds)
    {
        return Query.Where(R => Context.StudentCohorts.Any(SC => SC.StudentId == R.StudentId && CohortIds.Contains(SC.CohortId)));
    }

    /// <summary>
    /// SQL part of the version rule. Only previous-version answers (<c>LastVersion = false</c>) are filtered here;
    /// the "latest" case must not drop students, so it is resolved in memory with <see cref="LatestPerQuestion"/>.
    /// </summary>
    public static IQueryable<EvaluationAnswerRow> ForVersion(this IQueryable<EvaluationAnswerRow> Query, bool LastVersion)
    {
        return LastVersion
            ? Query
            : Query.Where(R => R.AnswerVersion != R.PollLastVersion);
    }

    /// <summary>
    /// Latest answer per student and question. A student imported before the poll's version went up only has
    /// older-version answers; comparing with <c>poll.LastVersion</c> would remove the student from the
    /// evaluation, so the newest version each student actually has is used instead.
    /// </summary>
    public static List<EvaluationAnswerRow> LatestPerQuestion(this IEnumerable<EvaluationAnswerRow> Rows)
    {
        return [.. Rows
            .GroupBy(R => new { R.StudentId, R.ComponentName, R.VariableName, R.Position })
            .Select(G => G.OrderByDescending(R => R.AnswerVersion).First())];
    }
}
