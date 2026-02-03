using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;

public class ErasEvaluationDetailsViewEntity
{
    public int EvaluationId { get; set; }
    public required string EvaluationName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required string Status { get; set; }
    public int PollId { get; set; }
    public required string PollName { get; set; }
    public required string PollUuid { get; set; }
    public int PollInstanceId { get; set; }
    public DateTime FinishedAt { get; set; }
    public int StudentId { get; set; }
    public required string StudentName { get; set; }
    public required string StudentEmail { get; set; }
    public int CohortId { get; set; }
    public int AnswerId { get; set; }
    public required string AnswerText { get; set; }
    public decimal RiskLevel { get; set; }
    public int VariableId { get; set; }
    public required string VariableName { get; set; }
    public int ComponentId { get; set; }
    public required string ComponentName { get; set; }
    public int VariableVersion { get; set; }
}
