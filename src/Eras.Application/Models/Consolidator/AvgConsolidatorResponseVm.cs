namespace Eras.Application.Models.Consolidator;
public class AvgConsolidatorResponseVm : ConsolidatorResponseVm
{
    public AvgConsolidatorResponseVm() : base()
    {
    }
}

public class AvgVariable : ReportVariable
{
    public double AverageRisk { get; set; }
}
