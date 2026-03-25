
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

public sealed class IndividualInterventionMapper : IMapper<IndividualInterventionDto, IndividualIntervention>
{
    public IndividualIntervention Map(IndividualInterventionDto source)
    {
        return new IndividualIntervention
        {
            Id = source.Id ?? Guid.NewGuid(),
            DateUtc = source.DateUtc,
            ActivityType = source.ActivityType,
            Professional = source.Professional,
            Comments = source.Comments,
            Attachments = source.Attachments
        };
    }
}
