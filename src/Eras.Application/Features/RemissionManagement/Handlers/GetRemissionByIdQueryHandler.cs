

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetRemissionByIdQueryHandler
    : IRequestHandler<GetRemissionByIdQuery, AssessmentDto?>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper;

    public GetRemissionByIdQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AssessmentDto?> Handle(
        GetRemissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        Assessment? entity = await _repository.GetByIdAsync(request.Id);

        return entity is null ? null : _mapper.Map(entity);
    }
}