using System.ComponentModel.DataAnnotations;

namespace Eras.Application.DTOs;

public class EvaluationDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage ="Name must be between 3 and 50 characters.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.DateTime)]
    public required DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.DateTime)]
    public required DateTime EndDate { get; set; }

    [StringLength(100, ErrorMessage = "Poll name must be less than 100 characters.")]
    public string PollName { get; set; } = string.Empty;

    [StringLength(10, ErrorMessage = "Country must be less than 10 characters.")]
    public string Country { get; set; } = string.Empty;
    public int EvaluationPollId { get; set; }
    public int PollId { get; set; }
    [Required(ErrorMessage = "Configuration Id is required.")]
    [Range(0, 2147483647, ErrorMessage = "Configuration Id must be greater or equal to 0.")]
    public int ConfigurationId { get; set; }

    [StringLength(30, ErrorMessage = "Status must be less than 30 characters.")]
    public string Status { get; set; } = String.Empty;

}
