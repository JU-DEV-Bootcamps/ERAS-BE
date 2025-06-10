namespace Eras.Application.Models.Consolidator;

public class RiskCountResponseVm
{
    public int AnswerCount { get; set; } = 0;
    public required decimal AverageRisk { get; set; }
    public IEnumerable<RiskRange> Risks { get; set; } = [];
}

public class RiskRange
{
    public string? Label { get; set; }
    public int StartRange { get; set; }
    public int EndRange { get; set; }
    public int Count { get; set; } = 0;
}
