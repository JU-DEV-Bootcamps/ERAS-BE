

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetStudentProfileByStudentCodeQueryHandler
    : IRequestHandler<GetStudentProfileByStudentCodeQuery, StudentProfileDto?>
{
    private readonly IStudentProfileRepository _repository;
    private readonly IMapper<StudentProfile, StudentProfileDto> _mapper;

    public GetStudentProfileByStudentCodeQueryHandler(
        IStudentProfileRepository repository,
        IMapper<StudentProfile, StudentProfileDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<StudentProfileDto?> Handle(
        GetStudentProfileByStudentCodeQuery request,
        CancellationToken cancellationToken)
    {
        StudentProfile? entity = await _repository.GetByStudentCodeAsync(request.StudentCode);

        return entity is null ? null : _mapper.Map(entity);
    }
}
