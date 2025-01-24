using System.Text.Json.Serialization;

namespace Eras.Application.DTOs
{
    public class StudentImportDto
    {
        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("Correo electronico")]
        public string CorreoElectronico { get; set; }

        [JsonPropertyName("Identificación de SIS del usuario")]
        public string IdentificacionDeSISDelUsuario { get; set; }

        [JsonPropertyName("Cursos inscritos:")]
        public int CursosInscritos { get; set; }

        [JsonPropertyName("Cursos con nota:")]
        public int CursosConNota { get; set; }

        [JsonPropertyName("Entregas a tiempo en comparación con todas")]
        public int EntregasATiempoEnComparacionConTodas { get; set; }

        [JsonPropertyName("Puntuación media")]
        public string PuntuacionMedia { get; set; }

        [JsonPropertyName("Cursos con una nota media por debajo de:")]
        public int CursosConUnaNotaMediaPorDebajoDe { get; set; }

        [JsonPropertyName("Diferencia de la puntuación pura")]
        public string DiferenciaDeLaPuntuacionPura { get; set; }

        [JsonPropertyName("Diferencia de la puntuación estandarizada")]
        public string DiferenciaDeLaPuntuacionEstandarizada { get; set; }

        [JsonPropertyName("Días desde el último acceso")]
        public int DiasDesdeElUltimoAcceso { get; set; }
    }
}
