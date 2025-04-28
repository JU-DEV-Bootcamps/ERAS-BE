using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.Response.HeatMap;
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

    [Column("variable_average_risk")]
    public decimal VariableAverageRisk { get; set; }

    [Column("percentage")]
    public decimal Percentage { get; set; }

}
