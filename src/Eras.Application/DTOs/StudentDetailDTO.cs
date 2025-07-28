using System.ComponentModel.DataAnnotations;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class StudentDetailDTO
{
    [Required(ErrorMessage = "Student Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Student Id must be greater than 0.")]
    public int StudentId { get; set; }
    [Range(0, 1000, ErrorMessage = "Enrolled courses must be between 0 and 1000.")]
    public int EnrolledCourses { get; set; }
    [Range(0, 1000, ErrorMessage = "Graded courses must be between 0 and 1000.")]
    public int GradedCourses { get; set; }
    [Range(0, 100, ErrorMessage = "Time delivery rate must be between 0 and 100.")]
    public int TimeDeliveryRate { get; set; }
    [Range(0, 100, ErrorMessage = "Average score must be between 0 and 100.")]
    public decimal AvgScore { get; set; }
    [Range(0, 1000, ErrorMessage = "Courses under average must be between 0 and 1000.")]
    public decimal CoursesUnderAvg { get; set; }
    [Range(-100, 100, ErrorMessage = "Pure score difference must be between -100 and 100.")]
    public decimal PureScoreDiff { get; set; }
    [Range(-5, 5, ErrorMessage = "Standard score difference must be between -5 and 5.")]
    public decimal StandardScoreDiff { get; set; }
    [Range(0, 9999, ErrorMessage = "Last access days must be between 0 and 9999.")]
    public int LastAccessDays { get; set; }
    public AuditInfo? Audit { get; set; } = default!;
}
