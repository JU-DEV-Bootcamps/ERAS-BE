using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers.QueryHandlers;
public class GetAssessmentsByCreatorQueryHandler(
    IAssessmentRepository Repository,
    IMapper<Assessment, AssessmentDto> Mapper,
    IStudentRepository StudentRepository
) : IRequestHandler<GetAssessmentsByCreatorQuery, IEnumerable<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository = Repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper = Mapper;
    private readonly IStudentRepository _studentRepository = StudentRepository;

    public async Task<IEnumerable<AssessmentDto>> Handle(
        GetAssessmentsByCreatorQuery request,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.creatorSub);

        IEnumerable<Assessment> entities = await _repository.GetByCreatorAsync(request.creatorSub);

        return await AssessmentStudentEnricher.EnrichWithStudentsAsync(
            entities, _mapper, _studentRepository, cancellationToken);
    }
}