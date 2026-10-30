using Eras.Domain.Entities;

namespace Eras.Domain.Tests.Entities;

public class StudentInterventionTypesTests
{
    [Theory]
    [InlineData(StudentInterventionType.Academic)]
    [InlineData(StudentInterventionType.Financial)]
    [InlineData(StudentInterventionType.Attendance)]
    [InlineData(StudentInterventionType.Personal)]
    public void IsValidFor_RiskType_OnlyBelongsToRiskIdentified(StudentInterventionType Type)
    {
        Assert.True(StudentInterventionTypes.IsValidFor(StudentInterventionKind.RiskIdentified, Type));
        Assert.False(StudentInterventionTypes.IsValidFor(StudentInterventionKind.SupportReceived, Type));
    }

    [Theory]
    [InlineData(StudentInterventionType.Tutoring)]
    [InlineData(StudentInterventionType.Advising)]
    [InlineData(StudentInterventionType.FinancialAid)]
    [InlineData(StudentInterventionType.Referral)]
    public void IsValidFor_SupportType_OnlyBelongsToSupportReceived(StudentInterventionType Type)
    {
        Assert.True(StudentInterventionTypes.IsValidFor(StudentInterventionKind.SupportReceived, Type));
        Assert.False(StudentInterventionTypes.IsValidFor(StudentInterventionKind.RiskIdentified, Type));
    }

    [Fact]
    public void For_EachKind_ListsFourTypes()
    {
        Assert.Equal(4, StudentInterventionTypes.For(StudentInterventionKind.RiskIdentified).Count);
        Assert.Equal(4, StudentInterventionTypes.For(StudentInterventionKind.SupportReceived).Count);
    }

    [Fact]
    public void IsValidFor_UndefinedType_IsNotValid()
    {
        Assert.False(StudentInterventionTypes.IsValidFor(StudentInterventionKind.RiskIdentified, (StudentInterventionType)999));
    }
}
