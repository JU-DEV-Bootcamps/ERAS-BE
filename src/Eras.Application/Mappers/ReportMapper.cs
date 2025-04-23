using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Mappers;
public static class ReportMapper
{
    public static AvgReportResponseVm MapToVmResponse(List<AnswersReportQueryResponse> QueryResponses)
    {
        List<AvgReportComponent> report = [.. QueryResponses
        .GroupBy(A => A.ComponentName)
        .Select(AnsPerComp => new AvgReportComponent
        {
            Description = AnsPerComp.Key.ToUpper(),
            AverageRisk = AnsPerComp.Average(Ans => Ans.RiskLevel),
            Questions = [.. AnsPerComp
                .OrderByDescending(Ans => Ans.RiskLevel)
                .GroupBy(A => A.Question)
                .Select(AnsPerVar => new AvgReportQuestions
                {
                    Question = AnsPerVar.Key,
                    AverageRisk = AnsPerVar.Average(Ans => Ans.RiskLevel),
                    Answer = AnsPerVar.First(A => A.RiskLevel <= AnsPerComp.Average(Ans => Ans.RiskLevel)).AnswerText ?? "No answer found.",
                })
                .OrderBy(Q => Q.AverageRisk)]
        })];
        var pollCount = QueryResponses.Select(A => A.PollInstanceId).Distinct().Count();
        return new AvgReportResponseVm { Components = report, PollCount = pollCount };

    }
}
