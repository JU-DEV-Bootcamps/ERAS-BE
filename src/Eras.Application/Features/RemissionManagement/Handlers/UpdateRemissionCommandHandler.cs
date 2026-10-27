

using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpdateRemissionCommandHandler
    : IRequestHandler<UpdateRemissionCommand, AssessmentDto>
{
    private readonly IMapper<AssessmentDto, Assessment> _toDomainMapper;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;
    private readonly IValidator<Assessment> _validator;
    private readonly IValidator<StatusTransitionRequest<AssessmentStatus>> _assessmentStatusValidator;
    private readonly IValidator<StatusTransitionRequest<InterventionStatus>> _interventionStatusValidator;
    private readonly IAssessmentRepository _repository;
    private readonly ILogger<UpdateRemissionCommandHandler> _logger;
    private readonly IErasUsersRepository _usersRepository;

    public UpdateRemissionCommandHandler(
        IMapper<AssessmentDto, Assessment> toDomainMapper,
        IMapper<Assessment, AssessmentDto> toDtoMapper,
        IValidator<Assessment> validator,
        IValidator<StatusTransitionRequest<AssessmentStatus>> statusValidator,
        IValidator<StatusTransitionRequest<InterventionStatus>> interventionStatusValidator,
        IAssessmentRepository repository,
        ILogger<UpdateRemissionCommandHandler> logger,
        IErasUsersRepository usersRepository)
    {
        _toDomainMapper = toDomainMapper;
        _toDtoMapper = toDtoMapper;
        _validator = validator;
        _assessmentStatusValidator = statusValidator;
        _interventionStatusValidator = interventionStatusValidator;
        _repository = repository;
        _logger = logger;
        _usersRepository = usersRepository;
    }

    public async Task<AssessmentDto> Handle(
        UpdateRemissionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Assessment entity = _toDomainMapper.Map(request.Remission);

            await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

            Assessment? existing = await _repository.GetByIdNoTrackingAsync(entity.Id, q => q.Include(a => a.Interventions));
            if (existing is null)
            {
                throw new KeyNotFoundException($"Assessment '{entity.Id}' not found.");
            }

            await ValidationHelper.ValidateAndThrowAsync(
                _assessmentStatusValidator,
                new StatusTransitionRequest<AssessmentStatus>(existing.Status, request.Remission.Status),
                cancellationToken);

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

            await EnsureRemovedProfessionalsHaveNoInterventionsAsync(existing, entity);

            var existingInterventionsById = existing.Interventions.ToDictionary(i => i.Id);

            foreach (var incoming in entity.Interventions)
            {
                if (existingInterventionsById.TryGetValue(incoming.Id, out var existingIntervention))
                {
                    await ValidationHelper.ValidateAndThrowAsync(
                        _interventionStatusValidator,
                        new StatusTransitionRequest<InterventionStatus>(existingIntervention.Status, incoming.Status),
                        cancellationToken);
                }
            }

            Assessment persisted = await _repository.UpdateAsync(entity);

            return _toDtoMapper.Map(persisted);
        }
        catch (ValidationException ex) 
        {
            _logger.LogWarning(ex, "Validation failed updating assessment {AssessmentId}.", request.Remission.Id);
            throw new OperationCanceledException($"Error updating assessment: Some status cannot be updated. ${ex.Message}");
        }
    }

    /// <summary>
    /// A professional can leave the assessment only if none of its interventions is assigned to them.
    /// An intervention holds the professional as free text, so it can carry the display name, the email
    /// or (in older rows) the sub; all of them count, ignoring case and surrounding spaces.
    /// </summary>
    private async Task EnsureRemovedProfessionalsHaveNoInterventionsAsync(Assessment existing, Assessment incoming)
    {
        List<string> removedSubs = existing.AssignedProfessionals.Except(incoming.AssignedProfessionals).ToList();

        foreach (string sub in removedSubs)
        {
            ErasUserDTO? user = await _usersRepository.GetErasUserBySubAsync(sub);

            string displayName = user is null ? sub : ProfessionalDisplayName.From(user);
            var identifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { sub, displayName };
            if (user is not null && !string.IsNullOrWhiteSpace(user.Email))
            {
                identifiers.Add(user.Email.Trim());
            }

            if (existing.Interventions.Any(I => !string.IsNullOrWhiteSpace(I.Professional) && identifiers.Contains(I.Professional.Trim())))
            {
                _logger.LogError("Cannot update assessment {AssessmentId}: professional {ProfessionalSub} still has interventions.", existing.Id, sub);
                throw new OperationCanceledException($"Error updating assessment: {displayName} cannot be removed. Since they still have interventions in this assessment.");
            }
        }
    }
}