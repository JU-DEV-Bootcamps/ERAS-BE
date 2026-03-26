
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

public sealed class InterventionPlanMapper : IMapper<InterventionPlanDto, InterventionPlan>
{
    public InterventionPlan Map(InterventionPlanDto source)
    {
        return new InterventionPlan
        {
            Id = source.Id ?? Guid.NewGuid(),
            SessionsPerWeek = source.SessionsPerWeek,
            ScheduleNotes = source.ScheduleNotes
        };
    }
}
