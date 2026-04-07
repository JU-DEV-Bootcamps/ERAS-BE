using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class InterventionPlanMapper : IMapper<InterventionPlanDto, InterventionPlan>
{
    public InterventionPlan Map(InterventionPlanDto source)
    {
        return new InterventionPlan
        {
            SessionsPerWeek = source.SessionsPerWeek,
            ScheduleNotes = source.ScheduleNotes
        };
    }
}
