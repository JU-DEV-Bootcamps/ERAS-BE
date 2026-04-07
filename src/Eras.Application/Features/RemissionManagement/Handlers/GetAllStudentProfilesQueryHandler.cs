

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetAllStudentProfilesQueryHandler
    : IRequestHandler<GetAllStudentProfilesQuery, IReadOnlyCollection<StudentProfileDto>>
{
    private readonly IStudentProfileRepository _repository;
    private readonly IMapper<StudentProfile, StudentProfileDto> _mapper;

    public GetAllStudentProfilesQueryHandler(
        IStudentProfileRepository repository,
        IMapper<StudentProfile, StudentProfileDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<StudentProfileDto>> Handle(
        GetAllStudentProfilesQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<StudentProfile> entities = await _repository.GetAllAsync();

        return entities.Select(_mapper.Map).ToArray();
    }
}