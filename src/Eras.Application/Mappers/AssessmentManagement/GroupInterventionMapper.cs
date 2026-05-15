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
            Comments = source.Comments,
            Activity = source.Activity,
            Area = source.Area,
            NumberOfGuests = source.NumberOfGuests,
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