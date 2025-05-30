namespace Eras.Domain.Entities
{
    public class StudentAnswer
    {
        public string Variable { get; set; } = string.Empty;
        public int Position { get; set; }
        public string Component { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
