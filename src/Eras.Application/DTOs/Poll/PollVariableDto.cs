using System.ComponentModel.DataAnnotations;

namespace Eras.Application.DTOs.Poll
{
    public class PollVariableDto
    {
        [Required(ErrorMessage = "Poll Id is required.")]
        [Range(1, 32767, ErrorMessage = "Poll Id must be greater or equal to 1.")]
        public int PollId { get; set; }

        [Required(ErrorMessage = "Variable Id is required.")]
        [Range(1, 32767, ErrorMessage = "Variable Id must be greater or equal to 1.")]
        public int VariableId { get; set; }
        public string VariableName { get; set; } = string.Empty;
    }
}
