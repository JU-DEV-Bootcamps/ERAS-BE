using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.Response.Calculations;
public class GetCohortTopRiskStudentsByComponentResponse
{
    [Column("student_id")]
    public int StudentId { get; set; }

    [Column("student_name")]
    public required string StudentName { get; set; }

    [Column("answer_average")]
    public decimal AnswerAverage { get; set; }

    [Column("risk_sum")]
    public decimal RiskSum { get; set; }
}
