using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Entities.FeatureFlagManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.FeatureFlags.Handlers.CommandHandlers;
public sealed class CreateFeatureFlagCommandHandler
    : IRequestHandler<CreateFeatureFlagCommand, FeatureFlagDTO>
{
    private readonly IFeatureFlagRepository _repository;
    private readonly IValidator<FeatureFlag> _validator;

    public CreateFeatureFlagCommandHandler(
        IFeatureFlagRepository Repository,
        IValidator<FeatureFlag> Validator)
    {
        _repository = Repository;
        _validator = Validator;
    }
    public async Task<FeatureFlagDTO> Handle(CreateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        FeatureFlag? existingEntity = await _repository.GetByNameAsync(request.FeatureFlag.Name);
        if (existingEntity != null)
            throw new InvalidOperationException($"Feature flag {existingEntity.Name} already exists.");
        FeatureFlagDTO dto = request.FeatureFlag;

        dto.Id = null;

        if (dto.Audit.CreatedAt == default)
            dto.Audit.CreatedAt = DateTime.UtcNow;

        if (string.IsNullOrEmpty(dto.Audit.CreatedBy))
            dto.Audit.CreatedBy = "System";

        FeatureFlag entity = dto.ToDomain();

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        FeatureFlag persisted = await _repository.AddAsync(entity);

        return persisted.ToDTO();
    }
}