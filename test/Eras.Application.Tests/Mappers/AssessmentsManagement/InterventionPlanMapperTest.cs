using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class InterventionPlanMapperTest
{
    [Fact]
    public void Map_ShouldMapInterventionPlan()
    {
        InterventionPlanDto source = new InterventionPlanDto()
        {
            SessionsPerWeek = 1,
            ScheduleNotes = "new intervention"
        };

        InterventionPlan result = CreateSut().Map(source);

        Assert.Equal(source.ScheduleNotes, result.ScheduleNotes);
        Assert.Equal(source.SessionsPerWeek, result.SessionsPerWeek);
    }

    private static InterventionPlanMapper CreateSut()
    {
        return new InterventionPlanMapper();
    }
}
