using Eras.Application.Models.Consolidator;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using static Eras.Application.Models.Consolidator.AvgConsolidatorResponseVm;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
public static class ConsolidatorMapper
{
    public static AvgConsolidatorResponseVm MapToAvg(IEnumerable<ComponentEntity> Components){
        var response = new AvgConsolidatorResponseVm
        {
            Components = Components.Select(C => new Component
            {
                Description = C.Name,
                Variables = C.Variables.Select(V => {
                    //Avg Answers?
                    IEnumerable<AnswerEntity> answers = V.PollVariables.SelectMany(PV => PV.Answers);
                    var averageRisk = answers.Average(A => A.RiskLevel);
                    return new AvgVariable
                    {
                        Question = V.Name,
                        //Get the answer text with the average risk level
                        Answer = answers.Where(A => A.RiskLevel == (int)Math.Round(averageRisk)).Select(A => A.AnswerText).FirstOrDefault() ?? "Something went wrong calculating the average risk. No answer found for that value",
                        Count = answers.Count(),
                        AverageRisk = averageRisk
                    };
                })
            })
        };
        return response;
    }
}
