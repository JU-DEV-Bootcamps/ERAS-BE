using Eras.Application.Models.Enums;

namespace Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

public class GetStudentsRecentAlertsResponse
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public RiskLevelEnum.RiskLevel RiskLevel { get; set; }
    public string Category { get; set; }
    public DateTime? Date { get; set; }
    public string Status { get; set; }
}
