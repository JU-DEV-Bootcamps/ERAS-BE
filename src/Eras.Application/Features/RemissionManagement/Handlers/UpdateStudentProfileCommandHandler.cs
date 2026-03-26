

using Eras.Application.Contracts.Persistence.RemissionManagement;
using Eras.Application.DTOs.RemissionManagement;
using Eras.Application.Mappers.RemissionManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.RemissionsManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpdateStudentProfileCommandHandler
    : IRequestHandler<UpdateStudentProfileCommand, StudentProfileDto>
{
    private readonly IMapper<StudentProfileDto, StudentProfile> _toDomainMapper;
    private readonly IMapper<StudentProfile, StudentProfileDto> _toDtoMapper;
    private readonly IValidator<StudentProfile> _validator;
    private readonly IStudentProfileRepository _repository;

    public UpdateStudentProfileCommandHandler(
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
        UpdateStudentProfileCommand request,
        CancellationToken cancellationToken)
    {
        StudentProfile entity = _toDomainMapper.Map(request.StudentProfile);

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        StudentProfile persisted = await _repository.UpdateAsync(entity);

        return _toDtoMapper.Map(persisted);
    }
}