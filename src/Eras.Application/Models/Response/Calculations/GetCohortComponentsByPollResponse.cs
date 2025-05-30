﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.Response.Calculations;
public class GetCohortComponentsByPollResponse
{
    [Column("cohort_id")]
    public int CohortId { get; set; }

    [Column("cohort_name")]
    public required string CohortName { get; set; }

    [Column("component_name")]
    public required string ComponentName { get; set; }

    [Column("average_risk_by_cohort_component")]
    public decimal? AverageRiskByCohortComponent { get; set; }

}
