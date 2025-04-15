namespace Eras.Application.Models.Consolidator;

public abstract class ConsolidatorResponseVm
{
    public required IEnumerable<ReportComponent> Components { get; set; } = [];
}

public class ReportComponent
{
    public required string Description { get; set; }
    public required IEnumerable<ReportVariable> Variables { get; set; } = [];

    public required double AverageRisk { get; set; }
}

public abstract class ReportVariable
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required int Count { get; set; }
}
