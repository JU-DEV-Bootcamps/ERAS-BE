

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.AssessmentManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class CreateStudentProfileCommandHandler
    : IRequestHandler<CreateStudentProfileCommand, StudentProfileDto>
{
    private readonly IMapper<StudentProfileDto, StudentProfile> _toDomainMapper;
    private readonly IMapper<StudentProfile, StudentProfileDto> _toDtoMapper;
    private readonly IValidator<StudentProfile> _validator;
    private readonly IStudentProfileRepository _repository;

    public CreateStudentProfileCommandHandler(
        IMapper<StudentProfileDto, StudentProfile> toDomainMapper,
        IMapper<StudentProfile, StudentProfileDto> toDtoMapper,
        IValidator<StudentProfile> validator,
        IStudentProfileRepository repository)
    {
        _toDomainMapper = toDomainMapper;
        _toDtoMapper = toDtoMapper;
        _validator = validator;
        _repository = repository;
    }

    public async Task<StudentProfileDto> Handle(
        CreateStudentProfileCommand request,
        CancellationToken cancellationToken)
    {
        StudentProfile entity = _toDomainMapper.Map(request.StudentProfile);

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        StudentProfile persisted = await _repository.AddAsync(entity);

        return _toDtoMapper.Map(persisted);
    }
}
