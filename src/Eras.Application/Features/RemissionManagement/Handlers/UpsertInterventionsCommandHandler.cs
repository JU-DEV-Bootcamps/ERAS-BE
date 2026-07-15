using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpsertInterventionsCommandHandler
    : IRequestHandler<UpsertInterventionsCommand, IReadOnlyCollection<InterventionDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<IndividualInterventionDto, IndividualIntervention> _individualMapper;
    private readonly IMapper<GroupInterventionDto, GroupIntervention> _groupMapper;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;
    private readonly IValidator<StatusTransitionRequest<InterventionStatus>> _interventionStatusValidator;

    public UpsertInterventionsCommandHandler(
        IAssessmentRepository repository,
        IMapper<IndividualInterventionDto, IndividualIntervention> individualMapper,
        IMapper<GroupInterventionDto, GroupIntervention> groupMapper,
        IMapper<Assessment, AssessmentDto> toDtoMapper,
        IValidator<StatusTransitionRequest<InterventionStatus>> interventionStatusValidator)
    {
        _repository = repository;
        _individualMapper = individualMapper;
        _groupMapper = groupMapper;
        _toDtoMapper = toDtoMapper;
        _interventionStatusValidator = interventionStatusValidator;
    }

    public async Task<IReadOnlyCollection<InterventionDto>> Handle(
        UpsertInterventionsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.AssessmentId);

            if (assessment is null)
                throw new KeyNotFoundException($"Assessment '{request.AssessmentId}' not found.");

            IReadOnlyCollection<Intervention> newInterventions = MapInterventions(request.Interventions);

            var existingInterventionsById = assessment.Interventions.ToDictionary(i => i.Id);

            foreach (var incoming in request.Interventions)
            {
                if (existingInterventionsById.TryGetValue((int)incoming.Id, out var existingIntervention))
                {
                    await ValidationHelper.ValidateAndThrowAsync(
                        _interventionStatusValidator,
                        new StatusTransitionRequest<InterventionStatus>(existingIntervention.Status, incoming.Status),
                        cancellationToken);
                }
            }
            await _repository.ReplaceInterventionsAsync(request.AssessmentId, newInterventions);

            return request.Interventions;
        }
        catch (ValidationException ex)
        {
            throw new OperationCanceledException($"Error updating assessment: Some status cannot be updated. ${ex.Message}");
        }
    }

    private IReadOnlyCollection<Intervention> MapInterventions(
        IReadOnlyCollection<InterventionDto> dtos)
    {
        List<Intervention> result = new(dtos.Count);

        foreach (InterventionDto dto in dtos)
        {
            Intervention mapped = dto switch
            {
                IndividualInterventionDto individual => _individualMapper.Map(individual),
                GroupInterventionDto group => _groupMapper.Map(group),
                _ => throw new NotSupportedException(
                    $"Intervention DTO type '{dto.GetType().Name}' is not supported.")
            };

            result.Add(mapped);
        }

        return result;
    }
}