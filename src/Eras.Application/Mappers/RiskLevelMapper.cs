using Eras.Application.DTOs.CL;
using Eras.Application.Models.Enums;

namespace Eras.Application.Mappers;

public static class RiskLevelMapper
{
    public static RiskLevelEnum.RiskLevel ToRiskLevel(double risk) => risk switch
    {
        >= 1 and <= 2 => RiskLevelEnum.RiskLevel.Low,
        > 2 and <= 3.5 => RiskLevelEnum.RiskLevel.Medium,
        > 3.5 and <= 5 => RiskLevelEnum.RiskLevel.High,
        _ => throw new ArgumentOutOfRangeException(nameof(risk))
    };
}
