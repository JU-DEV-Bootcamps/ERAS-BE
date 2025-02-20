namespace Eras.Application.Models.HeatMap
{
    public class HeatMapByVariablesResponseVm
    {
        public string ComponentName { get; set; } = string.Empty;
        public List<VariableResponse> Variables { get; set; } = new List<VariableResponse>();
    }

    public class VariableResponse
    {
        public string Name { get; set; } = string.Empty;
        public List<StudentData> Students { get; set; } = new List<StudentData>();
    }

    public class StudentData
    {
        public string Uuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RiskLevel { get; set; }
    }
}
