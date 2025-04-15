using Eras.Domain.Entities;

namespace Eras.Application.Models.Consolidator;

public class TopReportResponseVm
{
    public IEnumerable<TopReportComponent> Components { get; set; } = [];
}

public class TopReportComponent
{
    public required string Description { get; set; }
    public virtual IEnumerable<TopQuestionReport> Questions { get; set; } = [];

    public required double AverageRisk { get; set; }
}

public class TopQuestionReport
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required int Count { get; set; }
    public double TopRisk { get; set; }
    public List<Student> StudentsAtTopRisk { get; set; } = [];
}
