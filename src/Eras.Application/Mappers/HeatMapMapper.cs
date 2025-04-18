using Eras.Application.DTOs.CL;
using Eras.Application.Models.HeatMap;
namespace Eras.Application.Mappers
{
    public static class HeatMapMapper
    {
        public static HeatMapByComponentsResponseVm MaptToVmResponse(
            IEnumerable<GetHeatMapByComponentsQueryResponse> queryResponses,
            string componentName
        )
        {
            var variables = queryResponses
                .GroupBy(v => new { v.VariableId, v.VariableName })
                .Select(vg => new Variable
                {
                    VariableId = vg.Key.VariableId,
                    Description = vg.Key.VariableName,
                    PossibleAnswers = vg.Select(a => new PossibleAnswer
                    {
                        Description = a.AnswerText,
                        Value = a.AnswerRiskLevel
                    }).DistinctBy(pa => pa.Description)
                      .ToList()
                }).ToList();

            var maxAnswersCount = queryResponses
                .GroupBy(q => q.VariableId)
                .Max(g => g.Count());

            var series = queryResponses
                .GroupBy(q => new { q.VariableId, q.VariableName })
                .Select(g => new Series
                {
                    Name = g.Key.VariableName,
                    Data = g.GroupBy(a => a.AnswerText)
                            .Select(ag => new DataPoint
                            {
                                X = ag.Key ?? "No Answer",
                                Y = ag.Count()
                            })
                            .OrderBy(dp => dp.Y)
                            .ToList()
                })
                .ToList();

            foreach (var serie in series)
            {
                while (serie.Data.Count() < maxAnswersCount)
                {
                    serie.Data.Insert(0, new DataPoint { X = "No Answer", Y = -1 });
                }
            }

            return new HeatMapByComponentsResponseVm
            {
                ComponentName = componentName,
                Variables = new VariableData { Variables = variables },
                Series = series
            };
        }

        public static HeatMapSummaryResponseVm MapToSummaryVmResponse(
    IEnumerable<GetHeatMapByComponentsQueryResponse> queryResponses,
    IEnumerable<GetHeatMapAnswersPercentageByVariableQueryResponse> answersPercentage)
        {
            var components = queryResponses
                .GroupBy(q => new { q.ComponentId, q.ComponentName })
                .Select(cg => new Component
                {
                    Description = cg.Key.ComponentName.ToUpper(),
                    Variables = cg
                        .GroupBy(v => v.VariableId)
                        .Select(vg => new ComponentVars
                        {
                            Description = vg.First().VariableName,
                            AverageScore = vg.Average(v => v.AnswerRiskLevel)
                        })
                        .ToList()
                })
                .ToList();

            var series = components
                .Select(c => new SeriesSummary
                {
                    Name = c.Description,
                    Data = c.Variables
                        .Select(va => new DataPointSummary
                        {
                            X = va.Description, 
                            Y = Math.Round(Math.Min(va.AverageScore, 5), 2), 
                            Z = string.Join("\n", answersPercentage
                                .Where(ap => ap.Name == va.Description && ap.ComponentName?.ToUpper() == c.Description) 
                                .Select(ap => $"{ap.AnswerText} : {Math.Round(ap.Percentage, 2)}%")) 
                        })
                        .OrderBy(dp => dp.Y)
                        .ToList()
                })
                .ToList();

            return new HeatMapSummaryResponseVm
            {
                Components = components,
                Series = series
            };
        }
    }
}
