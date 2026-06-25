using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities.FeatureFlagManagement;

using MediatR;

namespace Eras.Application.Features.FeatureFlags.Handlers.CommandHandlers;

public class DeleteFeatureFlagCommandHandler : IRequestHandler<DeleteFeatureFlagCommand>
{
    private readonly IFeatureFlagRepository _repository;

    public DeleteFeatureFlagCommandHandler(IFeatureFlagRepository Repository)
    {
        _repository = Repository;
    }

    public async Task Handle(DeleteFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        FeatureFlag? entity = await _repository.GetByIdAsync(request.Id);
        if (entity is null)
            throw new KeyNotFoundException($"Feature Flag {request.Id} not found.");

        await _repository.DeleteAsync(entity);
    }
}