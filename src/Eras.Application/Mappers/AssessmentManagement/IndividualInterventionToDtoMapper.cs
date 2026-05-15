using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class IndividualInterventionToDtoMapper : IMapper<IndividualIntervention, IndividualInterventionDto>
{
    public IndividualInterventionDto Map(IndividualIntervention source)
    {
        return new IndividualInterventionDto
        {
            Id = source.Id,
            DateUtc = source.DateUtc,
            Activity = source.Activity,
            Area = source.Area,
            NumberOfGuests = source.NumberOfGuests,
            NumberOfParticipants = source.NumberOfParticipants,
            Professional = source.Professional,
            Comments = source.Comments,
            StudentIds = source.StudentIds,
            Attendance = source.Attendance,
            Mode = source.Mode,
            Status = source.Status,
            Remarks = source.Remarks,
            Attachments = source.Attachments
        };
    }
}