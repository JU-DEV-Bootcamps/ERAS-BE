using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class IndividualInterventionMapper : IMapper<IndividualInterventionDto, IndividualIntervention>
{
    public IndividualIntervention Map(IndividualInterventionDto source)
    {
        return new IndividualIntervention
        {
            Id = source.Id ?? default,
            DateUtc = source.DateUtc,
            Activity = source.Activity,
            Area = source.Area,
            NumberOfParticipants = source.NumberOfParticipants,
            Professional = source.Professional,
            StudentIds = source.StudentIds,
            Attendance = source.Attendance,
            Mode = source.Mode,
            Status = source.Status,
            Remarks = source.Remarks,
            Comments = source.Comments,
            Attachments = source.Attachments
        };
    }
}