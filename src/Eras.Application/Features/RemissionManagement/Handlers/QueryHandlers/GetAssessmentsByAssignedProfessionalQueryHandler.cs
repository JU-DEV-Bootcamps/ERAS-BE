using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers.QueryHandlers;
public class GetAssessmentsByAssignedProfessionalQueryHandler(
    IAssessmentRepository Repository,
    IMapper<Assessment, AssessmentDto> Mapper
) : IRequestHandler<GetAssessmentsByAssignedProfessionalQuery, IEnumerable<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository = Repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper = Mapper;

    public async Task<IEnumerable<AssessmentDto>> Handle(
        GetAssessmentsByAssignedProfessionalQuery request,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.assignedProfessionalSub);
        IEnumerable<Assessment> entities =
            await _repository.GetByAssignedProfessionalAsync(request.assignedProfessionalSub);

        return entities.Select(_mapper.Map);
    }
}