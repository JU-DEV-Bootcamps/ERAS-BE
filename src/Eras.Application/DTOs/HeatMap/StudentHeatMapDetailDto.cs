namespace Eras.Application.DTOs.HeatMap
{
    public class StudentHeatMapDetailDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int RiskLevel { get; set; }
        public string ComponentName { get; set; }
    }
}
