

using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.AssessmentManagement;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpdateRemissionCommandHandler
    : IRequestHandler<UpdateRemissionCommand, AssessmentDto>
{
    private readonly IMapper<AssessmentDto, Assessment> _toDomainMapper;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;
    private readonly IValidator<Assessment> _validator;
    private readonly IAssessmentRepository _repository;
    private readonly ILogger<UpdateRemissionCommandHandler> _logger;

    public UpdateRemissionCommandHandler(
        IMapper<AssessmentDto, Assessment> toDomainMapper,
        IMapper<Assessment, AssessmentDto> toDtoMapper,
        IValidator<Assessment> validator,
        IAssessmentRepository repository,
        ILogger<UpdateRemissionCommandHandler> logger)
    {
        _toDomainMapper = toDomainMapper;
        _toDtoMapper = toDtoMapper;
        _validator = validator;
        _repository = repository;
        _logger = logger;
    }

    public async Task<AssessmentDto> Handle(
        UpdateRemissionCommand request,
        CancellationToken cancellationToken)
    {
            Assessment entity = _toDomainMapper.Map(request.Remission);

            await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

            Assessment existing = await _repository.GetByIdNoTrackingAsync(entity.Id);

            var removedStudentIds = existing.StudentIds.Except(entity.StudentIds).ToList();

            if (removedStudentIds.Count > 0)
            {
                var affectedInterventions = await _repository.GetInterventionsContainingStudentAsync(existing, removedStudentIds);
                if (affectedInterventions.ToList().Count > 0)
                {
                    _logger.LogError("Cannot update assessment {AssessmentId}: student(s) {StudentIds} have related interventions.", entity.Id, string.Join(",", removedStudentIds));
                    throw new OperationCanceledException($"Error updating assessment: Some students cannot be removed. Since they have interventions related.");
                }
            }

            Assessment persisted = await _repository.UpdateAsync(entity);

            return _toDtoMapper.Map(persisted);
    }
}