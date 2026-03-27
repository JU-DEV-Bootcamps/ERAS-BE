using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

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
