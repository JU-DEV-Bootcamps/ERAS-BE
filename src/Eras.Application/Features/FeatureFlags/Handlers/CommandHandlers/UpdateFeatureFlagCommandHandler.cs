using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Entities.FeatureFlagManagement;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.FeatureFlags.Handlers.CommandHandlers;

public class UpdateFeatureFlagCommandHandler : IRequestHandler<UpdateFeatureFlagCommand, FeatureFlagDTO>
{
    private readonly IFeatureFlagRepository _repository;
    private readonly IValidator<FeatureFlag> _validator;

    public UpdateFeatureFlagCommandHandler(
        IFeatureFlagRepository Repository,
        IValidator<FeatureFlag> Validator
    )
    {
        _repository = Repository;
        _validator = Validator;
    }
    public async Task<FeatureFlagDTO> Handle(UpdateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        FeatureFlag? existingEntity = await _repository.GetByIdNoTrackingAsync(request.FeatureFlag.Id ?? default);
        if (existingEntity == null)
            throw new NotFoundException($"Feature Flag with ID ${request.FeatureFlag.Id} not found."); 

        FeatureFlagDTO dto = request.FeatureFlag;

        if (dto.Audit.ModifiedAt == default)
        {
            dto.Audit.ModifiedAt = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(dto.Audit.ModifiedBy))
        {
            dto.Audit.ModifiedBy = "System";
        }

        FeatureFlag entity = dto.ToDomain();
        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);
        FeatureFlag updatedEntity = await _repository.UpdateAsync(entity);

        return updatedEntity.ToDTO();
    }
}