
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

public sealed class GroupInterventionMapper : IMapper<GroupInterventionDto, GroupIntervention>
{
    public GroupIntervention Map(GroupInterventionDto source)
    {
        return new GroupIntervention
        {
            Id = source.Id ?? Guid.NewGuid(),
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
