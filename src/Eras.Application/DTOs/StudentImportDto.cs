using System.Text.Json.Serialization;
using Eras.Application.Utils;

namespace Eras.Application.DTOs
{
    public class StudentImportDto
    {
        [JsonPropertyName(nameof(Name))]
        public required string Name { get; set; }

        [JsonPropertyName(nameof(Email))]
        public required string Email { get; set; }

        [JsonPropertyName(nameof(SISId))]
        public required string SISId { get; set; }

        [JsonPropertyName(nameof(EnrolledCourses))]
        public int EnrolledCourses { get; set; }

        [JsonPropertyName(nameof(GradedCourses))]
        public int GradedCourses { get; set; }

        [JsonPropertyName(nameof(TimelySubmissions))]
        public int TimelySubmissions { get; set; }

        [JsonPropertyName(nameof(AverageScore))]
        public decimal AverageScore { get; set; }

        [JsonPropertyName(nameof(CoursesBelowAverage))]
        public int CoursesBelowAverage { get; set; }

        [JsonPropertyName(nameof(RawScoreDifference))]
        public decimal RawScoreDifference { get; set; }

        [JsonPropertyName(nameof(StandardScoreDifference))]
        public decimal StandardScoreDifference { get; set; }

        [JsonPropertyName(nameof(DaysSinceLastAccess))]
        public int DaysSinceLastAccess { get; set; }
    }
}
