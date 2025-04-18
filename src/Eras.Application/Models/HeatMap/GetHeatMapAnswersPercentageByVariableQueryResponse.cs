using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.HeatMap;
public class GetHeatMapAnswersPercentageByVariableQueryResponse
{
    [Column("component_name")]
    public string? ComponentName { get; set; }

    [Column("poll_variable_id")]
    public int PollVariableId { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("answer_text")]
    public string? AnswerText { get; set; }

    [Column("answer_count")]
    public int AnswerCount { get; set; }

    [Column("percentage")]
    public double Percentage { get; set; }

}
