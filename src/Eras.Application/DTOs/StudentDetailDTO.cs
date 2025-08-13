using System.ComponentModel.DataAnnotations;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class StudentDetailDTO
{
    [Required(ErrorMessage = "Student Id is required.")]
    [Range(1, 2147483647, ErrorMessage = "Student Id must be greater than 0.")]
    public int StudentId { get; set; }

    [Range(0, 32767, ErrorMessage = "Enrolled courses must be greater than 0.")]
    public int EnrolledCourses { get; set; }

    [Range(0, 32767, ErrorMessage = "Graded courses must be between 0 and 32767.")]
    public int GradedCourses { get; set; }

    [Range(0, 32767, ErrorMessage = "Time delivery rate must be between 0 and 32767.")]
    public int TimeDeliveryRate { get; set; }

    [Range(0, 18, ErrorMessage = "Average score must be between 0 and 18.")]
    public decimal AvgScore { get; set; }

    [Range(0, 18, ErrorMessage = "Courses under average must be between 0 and 18.")]
    public decimal CoursesUnderAvg { get; set; }

    [Range(0, 18, ErrorMessage = "Pure score difference must be between 0 and 18.")]
    public decimal PureScoreDiff { get; set; }

    [Range(0, 18, ErrorMessage = "Standard score difference must be between 0 and 18.")]
    public decimal StandardScoreDiff { get; set; }

    [Range(0, 32767, ErrorMessage = "Last access days must be between 0 and 32767.")]
    public int LastAccessDays { get; set; }

    public AuditInfo? Audit { get; set; } = default!;
}
