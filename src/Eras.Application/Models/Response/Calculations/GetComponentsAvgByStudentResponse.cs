using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Response.Calculations;
public class GetComponentsAvgByStudentResponse
{
    [Column("poll_instance_id")]
    public int? CohortId { get; set; }

    [Column("name")]
    public required string StudentName { get; set; }

    [Column("component_name")]
    public required string ComponentName { get; set; }

    [Column("student_component_average")]
    public decimal StudentComponentAverage { get; set; }
}
