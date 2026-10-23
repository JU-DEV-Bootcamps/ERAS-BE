using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class IndividualInterventionMapperTest
{
    [Fact]
    public void Map_ShouldMapIndividualIntervention()
    {
        IndividualInterventionDto source = new()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = new List<int>() { 1 },
            Activity = "New",
            NumberOfParticipants = 1,
            Area = "Academic",
            Status = InterventionStatus.Remitted
        };

        IndividualIntervention result = CreateSut().Map(source);

        Assert.Equal(source.DateUtc, result.DateUtc);
        Assert.Equal(source.StudentIds, result.StudentIds);
        Assert.Equal(source.NumberOfParticipants, result.NumberOfParticipants);
        Assert.Equal(source.Activity, result.Activity);
        Assert.Equal(source.Area, result.Area);
        Assert.Equal(source.Status, result.Status);
    }

    private static IndividualInterventionMapper CreateSut()
    {
        return new IndividualInterventionMapper();
    }
}

