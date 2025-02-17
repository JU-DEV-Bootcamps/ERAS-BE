using Eras.Application.Models.HeatMap;
namespace Eras.Application.Mappers
{
    public static class HeatMapMapper
    {
        public static HeatMapByComponentsResponseVm MaptToVmResponse(IEnumerable<GetHeatMapByComponentsQueryResponse> queryResponses)
        {

            var groupedByComponent = queryResponses
                .GroupBy(q => new { q.ComponentId, q.ComponentName })
                .Select(g => new VariableData
                {
                    SurveyKind = g.Key.ComponentName,
                    Variables = g.GroupBy(v => new { v.VariableId, v.VariableName })
                                 .Select(vg => new Variable
                                 {
                                     Description = vg.Key.VariableName,
                                     IsMultiple = vg.Count() > 1,
                                     PossibleAnswers = vg.Select(a => new PossibleAnswer
                                     {
                                         Description = a.AnswerText,
                                         Value = a.AnswerRiskLevel
                                     }).ToList()
                                 }).ToList()
                }).ToList();

            var groupedAnswersByComponent = queryResponses
                .GroupBy(q => new { q.ComponentId, q.ComponentName })
                .Select(g => new AnswerData
                {
                    SurveyKind = g.Key.ComponentName,
                    Answers = g.Select(a => new Answer
                    {
                        Description = a.AnswerText,
                        Value = a.AnswerRiskLevel
                    }).ToList()
                }).ToList();

            return new HeatMapByComponentsResponseVm
            {
                Variables = groupedByComponent,
                Answers = groupedAnswersByComponent
            };
        }
    }
}
