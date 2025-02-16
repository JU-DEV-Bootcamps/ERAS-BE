namespace Eras.Application.DTOs.HeatMapDTOs
{
    public class HeatMapComponentsDTO
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int VariableId { get; set; }
        public string VariableName { get; set; }
        public string AnswerText { get; set; }
        public int AnswerRiskLevel { get; set; }
    }
}
