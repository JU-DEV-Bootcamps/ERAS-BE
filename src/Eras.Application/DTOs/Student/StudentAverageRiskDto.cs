namespace Eras.Application.DTOs.Student
{
    public class StudentAverageRiskDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public double AvgRiskLevel { get; set; }
    }
}
