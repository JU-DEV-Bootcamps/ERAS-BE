using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.HeatMap
{
    public class GetHeatMapByComponentsQueryResponse
    {
        [Column("component_id")]
        public int ComponentId { get; set; }
        [Column("component_name")]
        public string? ComponentName { get; set; }
        [Column("variable_id")]
        public int VariableId { get; set; }
        [Column("variable_name")]
        public string? VariableName { get; set; }
        [Column("answer_text")]
        public string? AnswerText { get; set; }
        [Column("answer_risk_level")]
        public int AnswerRiskLevel { get; set; }
    }
}
