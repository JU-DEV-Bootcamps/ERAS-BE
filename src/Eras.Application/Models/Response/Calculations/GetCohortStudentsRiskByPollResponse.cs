using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Response.Calculations;
public class GetCohortStudentsRiskByPollResponse
{

    [Column("poll_instance_id")]
    public int PollInstanceId { get; set; }

    [Column("name")]
    public string StudentName { get; set; }

    [Column("poll_instance_risk_sum")]
    public decimal? PollInstanceRiskSum { get; set; }
}
