namespace Eras.Application.DTOs;

public class EvaluationDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public string PollName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int EvaluationPollId { get; set; }
    public int PollId { get; set; }
    public string Status { get; set; } = String.Empty;

}
