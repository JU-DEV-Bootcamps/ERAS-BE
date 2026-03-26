using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

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