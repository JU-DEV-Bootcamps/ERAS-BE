
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;


public class GroupInterventionToDtoMapperTest
{
    [Fact]
    public void Map_ShouldMapGroupInterventionToDto()
    {
        GroupIntervention source = new()
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = new List<int>() { 1, 2 },
            Activity = "New",
            NumberOfParticipants = 2,
            Area = "Academic",
            Status = InterventionStatus.Remitted
        };

        GroupInterventionDto result = CreateSut().Map(source);

        Assert.Equal(source.DateUtc, result.DateUtc);
        Assert.Equal(source.StudentIds, result.StudentIds);
        Assert.Equal(source.NumberOfParticipants, result.NumberOfParticipants);
        Assert.Equal(source.Activity, result.Activity);
        Assert.Equal(source.Area, result.Area);
        Assert.Equal(source.Status, result.Status);
    }

    private static GroupInterventionToDtoMapper CreateSut()
    {
        return new GroupInterventionToDtoMapper();
    }
}

