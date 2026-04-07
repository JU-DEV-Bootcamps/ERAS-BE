

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.AssessmentManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class CreateRemissionCommandHandler
    : IRequestHandler<CreateRemissionCommand, AssessmentDto>
{
    private readonly IMapper<AssessmentDto, Assessment> _toDomainMapper;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;
    private readonly IValidator<Assessment> _validator;
    private readonly IAssessmentRepository _repository;

    public CreateRemissionCommandHandler(
        IMapper<AssessmentDto, Assessment> toDomainMapper,
        IMapper<Assessment, AssessmentDto> toDtoMapper,
        IValidator<Assessment> validator,
        IAssessmentRepository repository)
    {
        _toDomainMapper = toDomainMapper;
        _toDtoMapper = toDtoMapper;
        _validator = validator;
        _repository = repository;
    }

    public async Task<AssessmentDto> Handle(
        CreateRemissionCommand request,
        CancellationToken cancellationToken)
    {
        Assessment entity = _toDomainMapper.Map(request.Remission);

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        Assessment persisted = await _repository.AddAsync(entity);

        return _toDtoMapper.Map(persisted);
    }
}