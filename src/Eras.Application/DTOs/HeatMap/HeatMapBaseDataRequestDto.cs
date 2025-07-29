using System.ComponentModel.DataAnnotations;

namespace Eras.Application.DTOs.HeatMap
{
    public class HeatMapBaseDataRequestDto
    {
        [Required(ErrorMessage = "Poll instance UUID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Poll instance UUID must be exactly 36 characters.")]
        [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "Poll instance UUID must follow a valid GUID format.")]

        public required string pollInstanceUuid { get; set; }

        [Required(ErrorMessage = "At least one variable ID is required.")]
        [MinLength(1, ErrorMessage = "The list of variable IDs cannot be empty.")]
        public required List<int> VariablesIds { get; set; }
    }
}
