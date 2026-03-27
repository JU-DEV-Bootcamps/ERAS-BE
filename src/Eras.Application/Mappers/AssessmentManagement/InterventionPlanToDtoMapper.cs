using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class InterventionPlanToDtoMapper : IMapper<InterventionPlan, InterventionPlanDto>
{
    public InterventionPlanDto Map(InterventionPlan source)
    {
        return new InterventionPlanDto
        {
            Id = source.Id,
            SessionsPerWeek = source.SessionsPerWeek,
            ScheduleNotes = source.ScheduleNotes
        };
    }
}