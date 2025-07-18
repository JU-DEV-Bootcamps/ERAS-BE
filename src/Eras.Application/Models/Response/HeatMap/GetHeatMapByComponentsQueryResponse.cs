﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Eras.Application.Models.Response.HeatMap;

public class GetHeatMapByComponentsQueryResponse
{
    [Column("component_id")]
    public int ComponentId { get; set; }
    [Column("component_name")]
    public string? ComponentName { get; set; }
    [Column("variable_id")]
    public int VariableId { get; set; }
    [Column("variable_name")]
    public required string VariableName { get; set; }
    [Column("answer_text")]
    public required string AnswerText { get; set; }
    [Column("answer_risk_level")]
    public decimal AnswerRiskLevel { get; set; }
}
