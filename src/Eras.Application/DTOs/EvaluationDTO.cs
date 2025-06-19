using System.ComponentModel.DataAnnotations;

namespace Eras.Application.DTOs;

public class EvaluationDTO
{
    public int Id { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public required DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public required DateTime EndDate { get; set; }

    [MaxLength(50)]
    public string PollName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Country { get; set; } = string.Empty;
    public int EvaluationPollId { get; set; }
    public int PollId { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = String.Empty;

}
