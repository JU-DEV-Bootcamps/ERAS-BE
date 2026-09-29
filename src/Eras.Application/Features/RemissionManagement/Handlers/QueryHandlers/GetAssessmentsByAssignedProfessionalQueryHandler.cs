using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers.QueryHandlers;
public class GetAssessmentsByAssignedProfessionalQueryHandler(
    IAssessmentRepository Repository,
    IMapper<Assessment, AssessmentDto> Mapper,
    IStudentRepository StudentRepository
) : IRequestHandler<GetAssessmentsByAssignedProfessionalQuery, IEnumerable<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository = Repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper = Mapper;
    private readonly IStudentRepository _studentRepository = StudentRepository;

    public async Task<IEnumerable<AssessmentDto>> Handle(
        GetAssessmentsByAssignedProfessionalQuery request,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.assignedProfessionalSub);
        IEnumerable<Assessment> entities =
            await _repository.GetByAssignedProfessionalAsync(request.assignedProfessionalSub);

        return await AssessmentStudentEnricher.EnrichWithStudentsAsync(
            entities, _mapper, _studentRepository, cancellationToken);
    }
}