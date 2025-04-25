namespace Eras.Application.DTOs.Poll
{
    public class PollVariableDto
    {
        public int PollId { get; set; }
        public int VariableId { get; set; }
        public string VariableName { get; set; } = string.Empty;
    }
}
