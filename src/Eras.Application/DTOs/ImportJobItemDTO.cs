namespace Eras.Application.DTOs
{
    public class ImportJobItemDTO
    {
        public int Id { get; set; }
        public int ImportJobId { get; set; }
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? Cohort { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RetryCount { get; set; }
        public bool IsAlreadyImported { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
