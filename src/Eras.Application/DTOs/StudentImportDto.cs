using System.Text.Json.Serialization;

namespace Eras.Application.DTOs
{
    public class StudentImportDto
    {
        [JsonPropertyName("Nombre")]
        public required string Name { get; set; }

        [JsonPropertyName("Correo electronico")]
        public required string Email { get; set; }

        [JsonPropertyName("Identificación de SIS del usuario")]
        public required string SISId { get; set; }

        [JsonPropertyName("Cursos inscritos")]
        public int EnrolledCourses { get; set; }

        [JsonPropertyName("Cursos con nota")]
        public int GradedCourses { get; set; }

        [JsonPropertyName("Entregas a tiempo en comparación con todas")]
        public int TimelySubmissions { get; set; }

        [JsonPropertyName("Puntuación media")]
        public decimal AverageScore { get; set; }

        [JsonPropertyName("Cursos con una nota media por debajo de")]
        public int CoursesBelowAverage { get; set; }

        [JsonPropertyName("Diferencia de la puntuación pura")]
        public decimal RawScoreDifference { get; set; }

        [JsonPropertyName("Diferencia de la puntuación estandarizada")]
        public decimal StandardScoreDifference { get; set; }

        [JsonPropertyName("Días desde el último acceso")]
        public int DaysSinceLastAccess { get; set; }
    }
}
