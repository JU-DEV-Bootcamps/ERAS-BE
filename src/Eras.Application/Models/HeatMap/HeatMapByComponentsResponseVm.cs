namespace Eras.Application.Models.HeatMap
{
    public class HeatMapByComponentsResponseVm
    {
        public required string ComponentName { get; set; }
        public VariableData Variables { get; set; } = new VariableData();
        public IEnumerable<Series> Series { get; set; } = new List<Series>();
    }

    public class VariableData
    {
        public IEnumerable<Variable> Variables { get; set; } = new List<Variable>();
    }

    public class Variable
    {
        public required string Description { get; set; }
        public IEnumerable<PossibleAnswer> PossibleAnswers { get; set; } = new List<PossibleAnswer>();
    }

    public class PossibleAnswer
    {
        public required string Description { get; set; }
        public int Value { get; set; }
    }

    public class Series
    {
        public required string Name { get; set; }
        public List<DataPoint> Data { get; set; } = new List<DataPoint>();
    }

    public class DataPoint
    {
        public required string X { get; set; }
        public int Y { get; set; }
    }
}
