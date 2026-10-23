using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class InterventionPlanToDtoMapperTest
{
    [Fact]
    public void Map_ShouldMapInterventionPlanToDto()
    {
        InterventionPlan source = new InterventionPlan()
        {
            SessionsPerWeek = 1,
            ScheduleNotes = "new intervention"
        };

        InterventionPlanDto result = CreateSut().Map(source);

        Assert.Equal(source.ScheduleNotes, result.ScheduleNotes);
        Assert.Equal(source.SessionsPerWeek, result.SessionsPerWeek);
    }

    private static InterventionPlanToDtoMapper CreateSut()
    {
        return new InterventionPlanToDtoMapper();
    }
}
