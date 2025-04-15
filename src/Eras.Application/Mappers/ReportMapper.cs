
using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;

public static class ReportMapper
{
    public static AvgReportResponseVm MaptToVmResponse(
        IEnumerable<Answer> PollAnswers
    )
    {
        var components = PollAnswers
            .GroupBy(A => new { A.Variable.Component.Id, A.Variable.Component.Name })
            .Select(AnsPerComponent =>
            {
                var averageRisk = Math.Round(AnsPerComponent.Average(A => A.RiskLevel), 2);
                return new AvgReportComponent
                {
                    Description = AnsPerComponent.Key.Name.ToUpper(),
                    AverageRisk = averageRisk,
                    Questions = [.. AnsPerComponent
                        .GroupBy(A => new { A.Variable.Id, A.AnswerText })
                        .Select(AnswersPerVar =>
                        {
                            var averageRisk = Math.Round(AnswersPerVar.Average(A => A.RiskLevel), 2);
                            var closestRisk = Math.Round(averageRisk, 0);
                            return new AvgReportQuestions {
                                Question = AnswersPerVar.FirstOrDefault(A => A.RiskLevel == closestRisk)?.Variable.Name ?? "Should be a valid variable name",
                                Answer = AnswersPerVar.FirstOrDefault(A => A.RiskLevel == closestRisk)?.AnswerText ?? "Should be a valid answer",
                                AverageRisk = averageRisk,
                                Count = AnsPerComponent.Count(),
                            };
                        })]
                };

            }).ToList();
        return new AvgReportResponseVm(){Components = components};
    }
}
