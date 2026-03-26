
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

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