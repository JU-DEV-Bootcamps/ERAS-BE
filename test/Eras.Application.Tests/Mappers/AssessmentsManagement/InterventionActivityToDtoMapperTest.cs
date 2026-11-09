using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Xunit;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class InterventionActivityToDtoMapperTest
{
    private readonly InterventionActivityToDtoMapper _sut = new();

    private static InterventionActivity MakeActivity(
        InterventionActivityType Type, InterventionLevel? Previous, InterventionLevel? New) => new()
        {
            Id = 1,
            Type = Type,
            PreviousRiskLevel = Previous,
            NewRiskLevel = New,
            OccurredAtUtc = new DateTime(2026, 11, 3, 10, 15, 0, DateTimeKind.Utc),
            ActorName = "Luis Gómez"
        };

    [Fact]
    public void Map_RiskLevelChanged_DescribesPreviousAndNewValue()
    {
        var result = _sut.Map(MakeActivity(InterventionActivityType.RiskLevelChanged, InterventionLevel.Medium, InterventionLevel.High));

        Assert.Equal("Risk level changed: Medium → High", result.Description);
        Assert.Equal("Luis Gómez", result.UserName);
    }

    [Fact]
    public void Map_EndRiskLevelChangedFromNone_DescribesNoneAsPreviousValue()
    {
        var result = _sut.Map(MakeActivity(InterventionActivityType.EndRiskLevelChanged, null, InterventionLevel.Low));

        Assert.Equal("End risk level changed: None → Low", result.Description);
    }
}
