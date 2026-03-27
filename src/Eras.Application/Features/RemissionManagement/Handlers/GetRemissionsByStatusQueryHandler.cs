

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetRemissionsByStatusQueryHandler
    : IRequestHandler<GetRemissionsByStatusQuery, IReadOnlyCollection<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper;

    public GetRemissionsByStatusQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<AssessmentDto>> Handle(
        GetRemissionsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Assessment> entities = await _repository.GetByStatusAsync(request.Status);

        return entities.Select(_mapper.Map).ToArray();
    }
}