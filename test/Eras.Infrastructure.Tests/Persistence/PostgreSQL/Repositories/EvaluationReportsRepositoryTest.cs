using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

/// <summary>
/// Avg / count / top reports read the answers of one evaluation directly (no whole-database view),
/// across every poll of that evaluation.
/// </summary>
public class EvaluationReportsRepositoryTest : RepositoryTestBase
{
    private const int Evaluation = 7;
    private const int OtherEvaluation = 8;

    private static readonly DateTime Start = DateTime.UtcNow.AddDays(-1);
    private static readonly DateTime End = DateTime.UtcNow.AddDays(1);

    private static AuditInfo Audit() => new() { CreatedAt = DateTime.UtcNow, CreatedBy = "test" };

    /// <summary>
    /// Evaluation 7 has two polls: poll A (student 1, and student 4 whose instance reuses student 1's answers)
    /// and poll B (student 2). "Q1" is a different variable/poll_variable in each poll. Student 3 belongs to evaluation 8.
    /// </summary>
    private static AppDbContext SeedContext(bool WithOldVersionAnswers = false)
    {
        var context = CreateContext();

        context.Components.Add(new ComponentEntity { Id = 1, Name = "Academic", Audit = Audit() });
        context.Polls.AddRange(
            new PollEntity { Id = 1, Uuid = "poll-a", Name = "Poll A", LastVersion = 1, Audit = Audit() },
            new PollEntity { Id = 2, Uuid = "poll-b", Name = "Poll B", LastVersion = 1, Audit = Audit() });
        context.Variables.AddRange(
            new VariableEntity { Id = 10, Name = "Q1", ComponentId = 1, Position = 1, Audit = Audit() },
            new VariableEntity { Id = 20, Name = "Q1", ComponentId = 1, Position = 1, Audit = Audit() },
            new VariableEntity { Id = 30, Name = "Q2", ComponentId = 1, Position = 2, Audit = Audit() });
        var version = new VersionInfo { VersionNumber = 1, VersionDate = DateTime.UtcNow };
        context.PollVariables.AddRange(
            new PollVariableJoin { Id = 1, PollId = 1, VariableId = 10, Version = version },
            new PollVariableJoin { Id = 2, PollId = 2, VariableId = 20, Version = version },
            new PollVariableJoin { Id = 3, PollId = 1, VariableId = 30, Version = version });

        context.Cohorts.Add(new CohortEntity { Id = 100, Name = "Cohort A", CourseCode = "C", Audit = Audit() });
        for (int id = 1; id <= 4; id++)
        {
            context.Students.Add(new StudentEntity { Id = id, Uuid = $"s{id}", Name = $"Student {id}", Email = $"s{id}@test.com" });
            context.StudentCohorts.Add(new StudentCohortJoin { StudentId = id, CohortId = 100 });
        }

        context.PollInstances.AddRange(
            new PollInstanceEntity { Id = 1, Uuid = "poll-a", StudentId = 1, EvaluationId = Evaluation, FinishedAt = DateTime.UtcNow, Audit = Audit() },
            new PollInstanceEntity { Id = 2, Uuid = "poll-b", StudentId = 2, EvaluationId = Evaluation, FinishedAt = DateTime.UtcNow, Audit = Audit() },
            new PollInstanceEntity { Id = 3, Uuid = "poll-a", StudentId = 3, EvaluationId = OtherEvaluation, FinishedAt = DateTime.UtcNow, Audit = Audit() },
            new PollInstanceEntity { Id = 4, Uuid = "poll-a", StudentId = 4, EvaluationId = Evaluation, FinishedAt = DateTime.UtcNow, SourcePollInstanceId = 1, Audit = Audit() });

        AnswerEntity Answer(int Id, int Instance, int PollVariable, string Text, decimal Risk, int VersionNumber = 1) => new()
        {
            Id = Id,
            PollInstanceId = Instance,
            PollVariableId = PollVariable,
            AnswerText = Text,
            RiskLevel = Risk,
            Audit = Audit(),
            Version = new VersionInfo { VersionNumber = VersionNumber, VersionDate = DateTime.UtcNow }
        };
        context.Answers.AddRange(
            Answer(1, 1, 1, "Good", 10),
            Answer(2, 1, 3, "Fine", 20),
            Answer(3, 2, 2, "Bad", 30),
            Answer(4, 3, 1, "Worst", 50));
        if (WithOldVersionAnswers)
        {
            context.Answers.Add(Answer(5, 2, 2, "Old", 40, VersionNumber: 0));
        }

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetReportByPollCohortAsync_IncludesEveryPollOfTheEvaluationAndNothingElseAsync()
    {
        await using var context = SeedContext();
        var repository = new PollInstanceRepository(context);

        var result = await repository.GetReportByPollCohortAsync(Evaluation, [100], true, Start, End);

        // students 1 and 4 (poll A, 4 via source instance) and 2 (poll B); student 3 is another evaluation
        Assert.Equal(3, result.PollCount);
        var component = Assert.Single(result.Components);
        Assert.Equal("ACADEMIC", component.Description);
        var q1 = Assert.Single(component.Questions, Q => Q.Question == "Q1");
        Assert.Equal(3, q1.AnswersDetails.Sum(D => D.StudentsEmails.Count()));
        // Q1 average is scoped to this evaluation: (10 + 30 + 10) / 3, the 50 of evaluation 8 is not included
        Assert.Equal(16.67m, q1.AverageRisk);
    }

    [Fact]
    public async Task GetReportByPollCohortAsync_WhenNotLastVersion_ReturnsOnlyOlderVersionAnswersAsync()
    {
        await using var context = SeedContext(WithOldVersionAnswers: true);
        var repository = new PollInstanceRepository(context);

        var result = await repository.GetReportByPollCohortAsync(Evaluation, [100], false, Start, End);

        Assert.Equal(1, result.PollCount);
        var details = Assert.Single(Assert.Single(Assert.Single(result.Components).Questions).AnswersDetails);
        Assert.Equal("s2@test.com", details.StudentsEmails.Single());
    }

    [Fact]
    public async Task GetReportByPollCohortAsync_WhenCohortDoesNotMatch_ReturnsEmptyAsync()
    {
        await using var context = SeedContext();
        var repository = new PollInstanceRepository(context);

        var result = await repository.GetReportByPollCohortAsync(Evaluation, [999], true, Start, End);

        Assert.Equal(0, result.PollCount);
        Assert.Empty(result.Components);
    }

    [Fact]
    public async Task GetCountReportByVariablesAsync_MatchesTheSameQuestionAcrossPollsAsync()
    {
        await using var context = SeedContext();
        var repository = new PollInstanceRepository(context);

        // only poll variable 1 (Q1 of poll A) is selected; Q1 of poll B must be matched by name
        var result = await repository.GetCountReportByVariablesAsync([100], [1], true, Start, End, Evaluation);

        var question = Assert.Single(Assert.Single(result.Components).Questions);
        Assert.Equal("Q1", question.Question);
        Assert.Equal(3, question.Answers.Sum(A => A.Count));
        Assert.Equal(16.67, question.AverageRisk, 2);
    }

    [Fact]
    public async Task GetByPollUuidVariableIdAsync_WithEvaluation_ReturnsTopAnswersOfAllPollsAsync()
    {
        await using var context = SeedContext();
        var repository = new PollVariableRepository(context);

        var result = await repository.GetByPollUuidVariableIdAsync(
            "poll-a", [10], new Pagination { Page = 0, PageSize = 10 }, Evaluation);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(["s2@test.com", "s1@test.com", "s4@test.com"], result.Items.Select(I => I.StudentEmail).ToList());
        Assert.DoesNotContain(result.Items, I => I.StudentEmail == "s3@test.com");
    }

    /// <summary>
    /// The in-memory provider accepts anything, so also check that the shared query translates to
    /// PostgreSQL (no connection is opened) and starts from the evaluation's poll instances.
    /// </summary>
    [Fact]
    public void EvaluationAnswerQuery_TranslatesToPostgresSql()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused")
            .Options;
        using var context = new AppDbContext(options);

        var sql = EvaluationAnswerQuery.Build(context, Evaluation, Start, End)
            .InCohorts(context, [100])
            .ForVersion(true)
            .Where(R => new List<string> { "Q1" }.Contains(R.VariableName))
            .ToQueryString();

        Assert.Contains("EvaluationId", sql);
        Assert.Contains("COALESCE", sql);
        Assert.Contains("EXISTS", sql);
    }
}
