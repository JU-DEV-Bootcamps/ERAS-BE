
using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.HeatMap
{
    public class GetHeatMapDetailByVariablesQueryResponse
    {
        [Column("student_uuid")]
        public string StudentUuid { get; set; } = string.Empty;
        [Column("student_name")]
        public string StudentName { get; set; } = string.Empty;
        [Column("component_name")]
        public string ComponentName { get; set; } = string.Empty;
        [Column("variable_name")]
        public string VariableName { get; set; } = string.Empty;
        [Column("answer_risk_level")]
        public int AnswerRiskLevel { get; set; }
        [Column("answer_name")]
        public string AnswerName { get; set; } = string.Empty;
    }
}
