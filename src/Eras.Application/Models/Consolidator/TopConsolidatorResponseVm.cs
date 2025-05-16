namespace Eras.Application.Models.Consolidator;

public class CountReportResponseVm
{
    public IEnumerable<CountReportComponent> Components { get; set; } = [];
}

public class CountReportComponent
{
    public required string Description { get; set; }
    public virtual IEnumerable<CountReportQuestion> Questions { get; set; } = [];

    public required double AverageRisk { get; set; }
}

public class CountReportQuestion
{
    public required string Question { get; set; }
    public required double AverageRisk { get; set; }
    public required IEnumerable<CountReportAnswer> Answers { get; set; }
}
public class CountReportAnswer
{
    public required int AnswerRisk { get; set; }
    public required int Count { get; set; }
    public IEnumerable<CountReportStudent> Students { get; set; } = [];
}

public class CountReportStudent
{
    public required string AnswerText { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int CohortId { get; set; }
    public string CohortName { get; set; } = "";
}
