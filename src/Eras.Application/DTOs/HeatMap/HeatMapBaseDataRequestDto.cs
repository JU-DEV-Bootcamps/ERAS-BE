using System.ComponentModel.DataAnnotations;

namespace Eras.Application.DTOs.HeatMap
{
    public class HeatMapBaseDataRequestDto
    {
        [Required]
        public required string pollInstanceUuid { get; set; }

        [Required]
        public required List<int> VariablesIds { get; set; }
    }
}
