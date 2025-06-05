namespace Eras.Application.Models.Response.HeatMap
{
    public class HeatMapSummaryResponseVm
    {
        public IEnumerable<Component> Components { get; set; } = new List<Component>();
        public IEnumerable<SeriesSummary> Series { get; set; } = new List<SeriesSummary>();
    }

    public class Component
    {
        public required string Description { get; set; }
        public IEnumerable<ComponentVars> Variables { get; set; } = new List<ComponentVars>();
    }

    public class ComponentVars
    {
        public required string Description { get; set; }
        public decimal AverageScore { get; set; }
    }

    public class SeriesSummary
    {
        public required string Name { get; set; }
        public List<DataPointSummary> Data { get; set; } = new List<DataPointSummary>();
    }

    public class DataPointSummary
    {
        public required string X { get; set; }
        public decimal Y { get; set; }
        public required string Z { get; set; }
    }
}
