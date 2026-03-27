using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class GroupInterventionToDtoMapper : IMapper<GroupIntervention, GroupInterventionDto>
{
    public GroupInterventionDto Map(GroupIntervention source)
    {
        return new GroupInterventionDto
        {
            Id = source.Id,
            DateUtc = source.DateUtc,
            ActivityType = source.ActivityType,
            Professional = source.Professional,
            Comments = source.Comments,
            Attachments = source.Attachments,
            Area = source.Area,
            ParticipantIds = source.ParticipantIds
        };
    }
}