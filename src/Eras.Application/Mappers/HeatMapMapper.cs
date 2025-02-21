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

        public static HeatMapByVariablesResponseVm MapToVmResponse(IEnumerable<GetHeatMapDetailByVariablesQueryResponse> queryResponses)
        {
            var groupedVariables = queryResponses
                .GroupBy(q => q.VariableName)
                .Select(vg => new VariableResponse
                {
                    Name = vg.Key,
                    Students = vg.Select(s => new StudentData
                    {
                        Uuid = s.StudentUuid,
                        Answer = s.AnswerName,
                        Name = s.StudentName,
                        RiskLevel = s.AnswerRiskLevel
                    })
                    .OrderByDescending(s => s.RiskLevel)
                    .ToList()
                }).ToList();

            return new HeatMapByVariablesResponseVm
            {
                ComponentName = queryResponses.FirstOrDefault()!.ComponentName,
                Variables = groupedVariables
            };
        }
        public static HeatMapSummaryResponseVm MapToSummaryVmResponse(
            IEnumerable<GetHeatMapByComponentsQueryResponse> queryResponses)
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
                        Y = Math.Round(Math.Min(va.AverageScore, 5), 2) // Limit the maxValue to 5, should be fixed
                    })
                    .OrderBy(dp => dp.Y)
                    .ToList()
                })
                .ToList();

            int maxDataCount = series
                .Max(s => s.Data.Count);

            foreach (var serie in series)
            {
                while (serie.Data.Count() < maxDataCount)
                {
                    serie.Data.Insert(0, new DataPointSummary { X = "No Answer", Y = -1.0 });
                }
            }


            return new HeatMapSummaryResponseVm
            {
                Components = components,
                Series = series
            };

        }
    }
}
