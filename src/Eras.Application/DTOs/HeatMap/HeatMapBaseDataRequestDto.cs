namespace Eras.Application.DTOs.HeatMap
{
    public class HeatMapBaseDataRequestDto
    {
        public required string pollInstanceUuid { get; set; }
        public required List<int> VariablesIds { get; set; }
    }
}
