using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.Response.HeatMap;
public class AnswersReportQueryResponse
{
    [Column("component_name")]
    public required string ComponentName { get; set; }

    [Column("poll_instance_id")]
    public int PollInstanceId { get; set; }

    [Column("question")]
    public required string Question { get; set; }

    [Column("answer_text")]
    public required string AnswerText { get; set; }

    [Column("risk_level")]
    public int RiskLevel { get; set; }

    [Column("student_email")]
    public required string StudentEmail { get; set; }

}
