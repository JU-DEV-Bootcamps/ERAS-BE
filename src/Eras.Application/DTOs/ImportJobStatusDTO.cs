namespace Eras.Application.DTOs
{
    public class ImportJobStatusDTO
    {
        public int ImportJobId { get; set; }
        public int EvaluationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int ExtractedCount { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public int ReturnedCount { get; set; }
        public int SkippedCount { get; set; }
        public int SkippedWithoutScore { get; set; }
        public int SkippedRequestFailed { get; set; }
        public int SkippedOutsideDateRange { get; set; }
        public int SkippedInvalidAnswers { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
