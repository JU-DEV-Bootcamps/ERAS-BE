
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

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