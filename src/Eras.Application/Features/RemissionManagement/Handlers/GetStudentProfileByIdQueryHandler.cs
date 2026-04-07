

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetStudentProfileByIdQueryHandler
    : IRequestHandler<GetStudentProfileByIdQuery, StudentProfileDto?>
{
    private readonly IStudentProfileRepository _repository;
    private readonly IMapper<StudentProfile, StudentProfileDto> _mapper;

    public GetStudentProfileByIdQueryHandler(
        IStudentProfileRepository repository,
        IMapper<StudentProfile, StudentProfileDto> mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<StudentProfileDto?> Handle(
        GetStudentProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        StudentProfile? entity = await _repository.GetByIdAsync(request.Id);

        return entity is null ? null : _mapper.Map(entity);
    }
}