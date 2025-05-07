using Eras.Application.Models.Response.HeatMap;
namespace Eras.Application.Mappers;

public static class HeatMapMapper
{
    public static HeatMapByComponentsResponseVm MaptToVmResponse(
        IEnumerable<GetHeatMapByComponentsQueryResponse> QueryResponses,
        string ComponentName
    )
    {
        var variables = QueryResponses
            .GroupBy(Var => new { Var.VariableId, Var.VariableName })
            .Select(VarGroup => new Variable
            {
                VariableId = VarGroup.Key.VariableId,
                Description = VarGroup.Key.VariableName,
                PossibleAnswers = [.. VarGroup.Select(Ans => new PossibleAnswer
                {
                    Description = Ans.AnswerText,
                    Value = Ans.AnswerRiskLevel
                }).DistinctBy(PAns => PAns.Description)]
            }).ToList();

        var maxAnswersCount = QueryResponses
            .GroupBy(VarG => VarG.VariableId)
            .Max(G => G.Count());

        var series = QueryResponses
            .GroupBy(QueryRes => new { QueryRes.VariableId, QueryRes.VariableName })
            .Select(G => new Series
            {
                Name = G.Key.VariableName,
                Data = [.. G.GroupBy(Ans => Ans.AnswerText)
                        .Select(AnsGr => new DataPoint
                        {
                            X = AnsGr.Key ?? "No Answer",
                            Y = AnsGr.Count()
                        })
                        .OrderBy(DataPoint => DataPoint.Y)]
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
            ComponentName = ComponentName,
            Variables = new VariableData { Variables = variables },
            Series = series
        };
    }

    public static HeatMapSummaryResponseVm MapToSummaryVmResponse(
        IEnumerable<GetHeatMapByComponentsQueryResponse> QueryResponses)
    {
        var components = QueryResponses
            .GroupBy(Q => new { Q.ComponentId, Q.ComponentName })
            .Select(CompGroup => new Component
            {
                Description = CompGroup.Key.ComponentName.ToUpper(),
                Variables = [.. CompGroup
                .GroupBy(Var => Var.VariableId)
                .Select(VarGr => new ComponentVars
                {
                    Description = VarGr.First().VariableName,
                    AverageScore = VarGr.Average(Var => Var.AnswerRiskLevel)
                })]
            })
            .ToList();

        var series = components
            .Select(Comp => new SeriesSummary
            {
                Name = Comp.Description,
                Data = [.. Comp.Variables
                .Select(Var => new DataPointSummary
                {
                    X = Var.Description,
                    Y = Math.Round(Math.Min(Var.AverageScore, 5), 2),
                    Z = ""
                })
                .OrderBy(DataPoint => DataPoint.Y)]
            })
            .ToList();
        var maxDataCount = series
            .Max(Series => Series.Data.Count);

        foreach (SeriesSummary? serie in series)
        {
            while (serie.Data.Count < maxDataCount)
            {
                serie.Data.Insert(0, new DataPointSummary { X = "No Answer", Y = -1.0, Z = "" });
            }
        }


        return new HeatMapSummaryResponseVm
        {
            Components = components,
            Series = series
        };

    }

    public static HeatMapSummaryResponseVm MapToSummaryAndPercentageVmResponse(
IEnumerable<GetHeatMapAnswersPercentageByVariableQueryResponse> QueryResponses)
    {
        var components = QueryResponses
            .GroupBy(Q => new { Q.ComponentName })
            .Select(CompGr => new Component
            {
                Description = CompGr.Key.ComponentName?.ToUpper(),
                Variables = [.. CompGr
                    .GroupBy(Var => Var.Name)
                    .Select(VarGr => new ComponentVars
                    {
                        Description = VarGr.Key,
                        AverageScore = (double) VarGr.First().VariableAverageRisk
                    })]
            })
            .ToList();

        var series = components
            .Select(Comp => new SeriesSummary
            {
                Name = Comp.Description,
                Data = [.. Comp.Variables
                    .Select(Var => new DataPointSummary
                    {
                        X = Var.Description,
                        Y = Math.Round(Math.Min(Var.AverageScore, 5), 2),
                        Z = string.Join("\n", QueryResponses
                            .Where(AnsPoint => AnsPoint.Name == Var.Description && AnsPoint.ComponentName?.ToUpper() == Comp.Description)
                            .Select(AP => $"{AP.AnswerText} : {Math.Round(AP.Percentage, 2)}%"))
                    })
                    .OrderBy(DataPoint => DataPoint.Y)]
            })
            .ToList();

        return new HeatMapSummaryResponseVm
        {
            Components = components,
            Series = series
        };
    }


}
