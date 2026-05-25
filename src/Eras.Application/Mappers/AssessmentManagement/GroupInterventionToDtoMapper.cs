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
            Comments = source.Comments,
            Activity = source.Activity,
            Area = source.Area,
            NumberOfParticipants = source.NumberOfParticipants,
            Professional = source.Professional,
            StudentIds = source.StudentIds,
            Attendance = source.Attendance,
            Mode = source.Mode,
            Status = source.Status,
            Remarks = source.Remarks,
            Attachments = source.Attachments
        };
    }
}