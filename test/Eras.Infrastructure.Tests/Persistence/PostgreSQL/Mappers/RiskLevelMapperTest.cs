using Eras.Application.Models.Enums;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers;

public class RiskLevelMapperTest
{
    [Fact]
    public void ToRiskLevel_Should_Convert_ToLowValue()
    {
        var result = RiskLevelMapper.ToRiskLevel(1.5);
        Assert.NotNull(result);
        Assert.Equal(RiskLevelEnum.RiskLevel.Low, result);
    }

    [Fact]
    public void ToRiskLevel_Should_Convert_ToMediumValue()
    {
        var result = RiskLevelMapper.ToRiskLevel(3.08);
        Assert.NotNull(result);
        Assert.Equal(RiskLevelEnum.RiskLevel.Medium, result);
    }

    [Fact]
    public void ToRiskLevel_Should_Convert_ToHighValue()
    {
        var result = RiskLevelMapper.ToRiskLevel(4.75);
        Assert.NotNull(result);
        Assert.Equal(RiskLevelEnum.RiskLevel.High, result);
    }

    [Fact]
    public void ToRiskLevel_Should_Throw_Exception()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RiskLevelMapper.ToRiskLevel(5.01));
    }
}
