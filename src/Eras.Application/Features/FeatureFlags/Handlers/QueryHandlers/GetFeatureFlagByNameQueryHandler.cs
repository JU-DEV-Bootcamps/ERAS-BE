using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Entities.FeatureFlagManagement;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.FeatureFlags.Handlers.QueryHandlers;
public class GetFeatureFlagByNameQueryHandler
    : IRequestHandler<GetFeatureFlagByNameQuery, FeatureFlagDTO?>
{
    private readonly IFeatureFlagRepository _repository;
    public GetFeatureFlagByNameQueryHandler(IFeatureFlagRepository Repository)
    {
        _repository = Repository;
    }

    public async Task<FeatureFlagDTO?> Handle(
        GetFeatureFlagByNameQuery request,
        CancellationToken cancellationToken
    )
    {
        FeatureFlag entity = await _repository.GetByNameAsync(request.Name)
            ?? throw new NotFoundException($"Feature Flag ${request.Name} not found");
        return entity.ToDTO();
    }
}