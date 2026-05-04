using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class GroupInterventionMapper : IMapper<GroupInterventionDto, GroupIntervention>
{
    public GroupIntervention Map(GroupInterventionDto source)
    {
        return new GroupIntervention
        {
            Id = source.Id ?? default,
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
