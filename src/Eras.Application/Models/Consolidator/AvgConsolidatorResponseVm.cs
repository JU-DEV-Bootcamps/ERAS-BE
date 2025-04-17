﻿namespace Eras.Application.Models.Consolidator;

public class AvgReportResponseVm
{
    public int PollCount { get; set; } = 0;
    public IEnumerable<AvgReportComponent> Components { get; set; } = [];
}

public class AvgReportComponent
{
    public required string Description { get; set; }
    public virtual IEnumerable<AvgReportQuestions> Questions { get; set; } = [];

    public required double AverageRisk { get; set; }
}

public class AvgReportQuestions
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public double AverageRisk { get; set; }
}
