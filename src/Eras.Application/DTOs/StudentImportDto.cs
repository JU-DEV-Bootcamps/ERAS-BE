using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Eras.Application.DTOs;

public class StudentImportDto
{
    public string? Cohort { get; set; }

    [StringLength(255, ErrorMessage = "Name must not exceed 255 characters.")]
    [JsonPropertyName(nameof(Name))]
    public required string Name { get; set; }

    [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
    [JsonPropertyName(nameof(Email))]
    public required string Email { get; set; }

    [JsonPropertyName(nameof(SISId))]
    public required string SISId { get; set; }

    [Range(-32768, 32767, ErrorMessage = "Enrolled courses must be between -32768 and 32767")]
    [JsonPropertyName(nameof(EnrolledCourses))]
    public int EnrolledCourses { get; set; }

    [Range(-32768, 32767, ErrorMessage = "Enrolled courses must be between -32768 and 32767")]
    [JsonPropertyName(nameof(GradedCourses))]
    public int GradedCourses { get; set; }

    [Range(-32768, 32767, ErrorMessage = "Enrolled courses must be between -32768 and 32767")]
    [JsonPropertyName(nameof(TimelySubmissions))]
    public int TimelySubmissions { get; set; }

    [Range(typeof(decimal), "0.0000", "999999999999.9999", ErrorMessage = "Value must be between 0.0000 and 999999999999.9999.")]
    [JsonPropertyName(nameof(AverageScore))]
    public decimal AverageScore { get; set; }

    [JsonPropertyName(nameof(CoursesBelowAverage))]
    public int CoursesBelowAverage { get; set; }

    [JsonPropertyName(nameof(RawScoreDifference))]
    public decimal RawScoreDifference { get; set; }

    [Range(typeof(decimal), "0.0000", "999999999999.9999", ErrorMessage = "Value must be between 0.0000 and 999999999999.9999.")]
    [JsonPropertyName(nameof(StandardScoreDifference))]
    public decimal StandardScoreDifference { get; set; }

    [Range(0, 32767, ErrorMessage = "Days since last access must be a non-negative number between 0 and 32767.")]
    [JsonPropertyName(nameof(DaysSinceLastAccess))]
    public int DaysSinceLastAccess { get; set; }
}
