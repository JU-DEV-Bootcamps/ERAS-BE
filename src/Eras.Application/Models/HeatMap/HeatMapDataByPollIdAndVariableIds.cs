namespace Eras.Application.Models.HeatMap
{
    public class HeatMapBaseData
    {
        public required string Name { get; set; }
        public required List<Serie> Data { get; set; }
    }

    public class Serie
    {
        public required string x;
        public required int y;
    }
}
