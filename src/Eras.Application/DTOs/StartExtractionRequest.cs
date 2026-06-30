namespace Eras.Application.DTOs
{
    public class StartExtractionRequest
    {
        public string EvaluationSetName { get; set; } = string.Empty;
        public int ConfigurationId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int EvaluationId { get; set; }
    }

    public class ConfirmImportRequest
    {
        public List<int> ItemIds { get; set; } = [];
    }
}
