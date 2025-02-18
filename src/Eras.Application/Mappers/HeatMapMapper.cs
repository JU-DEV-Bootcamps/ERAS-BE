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
            var filteredResponses = queryResponses
                .Where(q => q.ComponentName == componentName)
                .ToList();

            var groupedByComponent = filteredResponses
                .GroupBy(q => new { q.ComponentId, q.ComponentName })
                .Select(g => new VariableData
                {
                    Variables = g.GroupBy(v => new { v.VariableId, v.VariableName })
                                 .Select(vg => new Variable
                                 {
                                     Description = vg.Key.VariableName,
                                     PossibleAnswers = vg.Select(a => new PossibleAnswer
                                     {
                                         Description = a.AnswerText,
                                         Value = a.AnswerRiskLevel
                                     }).DistinctBy(pa => new { pa.Description, pa.Value }).ToList()
                                 }).ToList()
                }).ToList();

            var answers = filteredResponses
                .GroupBy(q => new { q.ComponentId, q.ComponentName })
                .Select(g => new AnswerData
                {
                    Answers = g.Select(a => new Answer
                    {
                        Description = a.AnswerText,
                        Value = a.AnswerRiskLevel
                    }).ToList()
                }).ToList();

            return new HeatMapByComponentsResponseVm
            {
                ComponentName = componentName,
                Variables = groupedByComponent,
                Answers = answers
            };
        }
    }
}
