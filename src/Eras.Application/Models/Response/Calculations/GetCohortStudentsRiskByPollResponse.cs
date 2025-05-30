﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.Response.Calculations;
public class GetCohortStudentsRiskByPollResponse
{

    [Column("poll_instance_id")]
    public int PollInstanceId { get; set; }

    [Column("name")]
    public required string StudentName { get; set; }

    [Column("poll_instance_risk_sum")]
    public decimal PollInstanceRiskSum { get; set; }
}
