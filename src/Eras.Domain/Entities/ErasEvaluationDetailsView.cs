using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities;

public class ErasEvaluationDetailsView
{
    public int EvaluationId { get; set; }
    public string EvaluationName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PollId { get; set; }
    public string PollName { get; set; } = string.Empty;
    public string PollUuid { get; set; } = string.Empty;
    public int? PollInstanceId { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public int? CohortId { get; set; }
    public int? AnswerId { get; set; }
    public string? AnswerText { get; set; }
    public decimal? RiskLevel { get; set; }
    public int? VariableId { get; set; }
    public string? VariableName { get; set; }
    public int? ComponentId { get; set; }
    public string? ComponentName { get; set; }
    public int? VariableVersion { get; set; }
}
