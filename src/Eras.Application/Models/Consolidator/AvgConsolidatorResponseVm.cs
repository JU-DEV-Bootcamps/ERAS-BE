namespace Eras.Application.Models.Consolidator;

public class AvgReportResponseVm
{
    public int PollCount { get; set; } = 0;
    public IEnumerable<AvgReportComponent> Components { get; set; } = [];
}

public class AvgReportComponent
{
    public required string Description { get; set; }
    public virtual IEnumerable<AvgReportQuestions> Questions { get; set; } = [];

    public required decimal AverageRisk { get; set; }
}

public class AvgReportQuestions
{
    public required string Question { get; set; }
    public required string AverageAnswer { get; set; }
    public decimal AverageRisk { get; set; }

    public IEnumerable<AnswerDetails> AnswersDetails { get; set; } = [];
}

public class AnswerDetails
{
    public required string AnswerText { get; set; }
    public decimal AnswerPercentage { get; set; }
    public IEnumerable<string> StudentsEmails { get; set; } = [];
}
