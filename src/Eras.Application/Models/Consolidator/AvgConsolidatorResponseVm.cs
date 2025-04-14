namespace Eras.Application.Models.Consolidator;
public class AvgConsolidatorResponseVm : ConsolidatorResponseVm
{
    public IEnumerable<AvgVariable> Variables { get; set; } = [];

    public class AvgVariable : Variable
    {
        public double AverageRisk { get; set; }
    }
}
