using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Response.Calculations;
public class GetCohortComponentsByPollResponse
{
    [Column("cohort_id")]
    public int? CohortId { get; set; }

    [Column("cohort_name")]
    public string? CohortName { get; set; }

    [Column("component_name")]
    public string ComponentName { get; set; }

    [Column("average_risk_by_cohort_component")]
    public decimal? AverageRiskByCohortComponent { get; set; }

}
