using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities.AssessmentManagement;
using MediatR;
using Eras.Domain.Entities;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetAllRemissionsQueryHandler
    : IRequestHandler<GetAllRemissionsQuery, IReadOnlyCollection<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper;
    private readonly IStudentRepository _studentRepository;

    public GetAllRemissionsQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> mapper,
        IStudentRepository studentRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _studentRepository = studentRepository;
    }

    public async Task<IReadOnlyCollection<AssessmentDto>> Handle(
        GetAllRemissionsQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Assessment> entities = await _repository.GetAllAsync();

        return await AssessmentStudentEnricher.EnrichWithStudentsAsync(
            entities, _mapper, _studentRepository, cancellationToken);
    }
}