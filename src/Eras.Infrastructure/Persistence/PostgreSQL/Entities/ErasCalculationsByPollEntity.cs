namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;

public class ErasCalculationsByPollEntity
{
    public int PollId { get; set; }
    public required string PollUuid { get; set; }
    public int ComponentId { get; set; }
    public required string ComponentName { get; set; }
    public int PollVariableId { get; set; }
    public required string Question { get; set; }
    public required string AnswerText { get; set; }
    public int PollInstanceId { get; set; }
    public int PollInstanceRiskSum { get; set; }
    public int PollInstanceAnswersCount { get; set; }
    public required string StudentName { get; set; }
    public required string StudentEmail { get; set; }
    public decimal AnswerRisk { get; set; }
    public decimal RiskCount { get; set; }
    public decimal AverageRisk { get; set; }
    public decimal ComponentAverageRisk { get; set; }
    public decimal VariableAverageRisk { get; set; }
    public int AnswerCount { get; set; }
    public decimal AnswerPercentage { get; set; }
    public int CohortId { get; set; }
    public required string CohortName { get; set; }
    public decimal AverageRiskByCohortComponent { get; set; }
    public int StudentId { get; set; }
    public int PollVersion { get; set; }
}
