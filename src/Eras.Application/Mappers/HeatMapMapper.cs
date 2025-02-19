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

            var variableData = new VariableData
            {
                Variables = queryResponses
                    .GroupBy(v => new { v.VariableId, v.VariableName })
                    .Select(vg => new Variable
                    {
                        Description = vg.Key.VariableName,
                        PossibleAnswers = vg.Select(a => new PossibleAnswer
                        {
                            Description = a.AnswerText,
                            Value = a.AnswerRiskLevel
                        }).GroupBy(pa => pa.Description)
                          .Select(gpa => gpa.First())
                          .ToList()
                    }).ToList()
            };

            var answerData = new AnswerData
            {
                Answers = queryResponses
                    .Select(a => new Answer
                    {
                        Description = a.AnswerText,
                        Value = a.AnswerRiskLevel
                    }).ToList()
            };

            return new HeatMapByComponentsResponseVm
            {
                ComponentName = componentName,
                Variables = variableData,
                Answers = answerData
            };
        }
    }
}
