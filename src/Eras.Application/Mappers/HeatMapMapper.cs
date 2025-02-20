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
    }
}
