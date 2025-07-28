using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Eras.Application.DTOs;

public class StudentImportDto
{
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Cohort must be between 3 and 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Cohort can only contain letters, numbers, spaces, and hyphens.")]
    public string? Cohort { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
    [JsonPropertyName(nameof(Name))]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    [JsonPropertyName(nameof(Email))]
    public required string Email { get; set; }

    [Required(ErrorMessage = "SIS ID is required.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "SIS ID must be between 3 and 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "SIS ID can only contain letters, numbers, and hyphens.")]
    [JsonPropertyName(nameof(SISId))]
    public required string SISId { get; set; }

    [Range(0, 1000, ErrorMessage = "Enrolled courses must be between 0 and 1000.")]
    [JsonPropertyName(nameof(EnrolledCourses))]
    public int EnrolledCourses { get; set; }

    [Range(0, 1000, ErrorMessage = "Graded courses must be between 0 and 1000.")]
    [JsonPropertyName(nameof(GradedCourses))]
    public int GradedCourses { get; set; }

    [Range(0, 1000, ErrorMessage = "Timely submissions must be between 0 and 1000.")]
    [JsonPropertyName(nameof(TimelySubmissions))]
    public int TimelySubmissions { get; set; }

    [Range(0, 100, ErrorMessage = "Average score must be between 0 and 100.")]
    [JsonPropertyName(nameof(AverageScore))]
    public decimal AverageScore { get; set; }

    [Range(0, 1000, ErrorMessage = "Courses below average must be between 0 and 1000.")]
    [JsonPropertyName(nameof(CoursesBelowAverage))]
    public int CoursesBelowAverage { get; set; }

    [Range(-100, 100, ErrorMessage = "Raw score difference must be between -100 and 100.")]
    [JsonPropertyName(nameof(RawScoreDifference))]
    public decimal RawScoreDifference { get; set; }

    [Range(-5, 5, ErrorMessage = "Standard score difference must be between -5 and 5.")]
    [JsonPropertyName(nameof(StandardScoreDifference))]
    public decimal StandardScoreDifference { get; set; }

    [Range(0, 9999, ErrorMessage = "Days since last access must be between 0 and 9999.")]
    [JsonPropertyName(nameof(DaysSinceLastAccess))]
    public int DaysSinceLastAccess { get; set; }
}
