namespace Eras.Domain.Entities
{
    public class Answer : BaseEntity
    {
        public string AnswerText { get; set; } = string.Empty;
        public int RiskLevel { get; set; }
        public int PollInstanceId { get; set; }
        public PollInstance PollInstance { get; set; } = default!;
    }
}
