using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class IndividualInterventionToDtoMapper
    : IMapper<IndividualIntervention, IndividualInterventionDto>
{
    public IndividualInterventionDto Map(IndividualIntervention source)
    {
        return new IndividualInterventionDto
        {
            Id = source.Id,
            DateUtc = source.DateUtc,
            ActivityType = source.ActivityType,
            Professional = source.Professional,
            Comments = source.Comments,
            Attachments = source.Attachments
        };
    }
}