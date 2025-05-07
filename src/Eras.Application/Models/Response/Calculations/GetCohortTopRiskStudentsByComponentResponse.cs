using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Response.Calculations;
public class GetCohortTopRiskStudentsByComponentResponse
{
    [Column("student_id")]
    public int StudentId { get; set; }

    [Column("student_name")]
    public required string StudentName { get; set; }

    [Column("risk_sum")]
    public decimal RiskSum { get; set; }
}
