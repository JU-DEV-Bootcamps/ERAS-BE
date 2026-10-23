using Eras.Application.Models.Enums;

namespace Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

public class GetStudentsRecentAlertsResponse
{
    public required string StudentId { get; set; }
    public required string StudentName { get; set; }
    public RiskLevelEnum.RiskLevel RiskLevel { get; set; }
    public required string Category { get; set; }
    public required DateTime? Date { get; set; }
    public required string Status { get; set; }
}
